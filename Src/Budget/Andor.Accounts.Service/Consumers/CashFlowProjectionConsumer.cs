using Andor.Accounts.Application;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.ValuesObjects;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Accounts.Service.Consumers;

internal sealed record CashFlowMovementMessage(
    Guid Id,
    Guid FinancialMovementId,
    DateTime Date,
    decimal Value,
    int Status,
    int Type);

internal sealed record CashFlowMovementEditedMessage(
    Guid Id,
    Guid FinancialMovementId,
    DateTime Date,
    int Type,
    decimal PreviousValue,
    int PreviousStatus,
    decimal Value,
    int Status);

public sealed class CashFlowProjectionSubscriptionOptions
{
    public const string SectionName = "CashFlowProjectionSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the module's "andor-accounts-events" topic and maintains the CashFlow monthly
/// balance projection. Runs independently of FinancialMovementCreatedConsumer — both react
/// to the same event, and this one needs nothing beyond the event's own payload.
///
/// Reacts to Added (apply), Removed (reverse), and Edited (reverse-then-apply, since edits are
/// only fired in place when the movement's month and type didn't change) so the projection stays
/// consistent through the whole financial-movement lifecycle, not just creation.
///
/// Because this app is a personal ledger (not a bank), a movement can land in a past month,
/// so applying it also cascades forward through every later month that already has a
/// CashFlow row for the account, re-anchoring each one's opening balance in turn.
/// </summary>
public sealed class CashFlowProjectionConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CashFlowProjectionConsumer> _logger;

    public CashFlowProjectionConsumer(
        ServiceBusClient client,
        IOptions<CashFlowProjectionSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<CashFlowProjectionConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "CashFlowProjectionSubscription:TopicName and SubscriptionName must be configured.");
        }

        _processor = client.CreateProcessor(options.Value.TopicName, options.Value.SubscriptionName);
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await _processor.StopProcessingAsync(CancellationToken.None);
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var subject = args.Message.Subject;

        if (subject != nameof(AccountFinancialMovementAddedDomainEvent) &&
            subject != nameof(AccountFinancialMovementRemovedDomainEvent) &&
            subject != nameof(AccountFinancialMovementEditedDomainEvent))
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var cashFlowRepo = scope.ServiceProvider.GetRequiredService<ICommandsCashFlowRepository>();
        var webSocketMessage = scope.ServiceProvider.GetRequiredService<IWebSocketMessage>();

        if (subject == nameof(AccountFinancialMovementEditedDomainEvent))
        {
            var edited = args.Message.Body.ToObjectFromJson<CashFlowMovementEditedMessage>();
            var accountId = AccountId.Load(edited.Id);
            var type = Enumeration<int>.GetByKey<MovementType>(edited.Type);

            // Reverse the movement's contribution under its previous value/status, then apply it
            // again under the new one. Both land in the same period/type bucket, since Edited is
            // only ever fired for in-place edits (month and type unchanged).
            await ApplyDeltaAsync(cashFlowRepo, webSocketMessage, accountId, edited.Date, type,
                Enumeration<int>.GetByKey<MovementStatus>(edited.PreviousStatus), -edited.PreviousValue,
                args.CancellationToken);

            await ApplyDeltaAsync(cashFlowRepo, webSocketMessage, accountId, edited.Date, type,
                Enumeration<int>.GetByKey<MovementStatus>(edited.Status), edited.Value,
                args.CancellationToken);
        }
        else
        {
            var message = args.Message.Body.ToObjectFromJson<CashFlowMovementMessage>();
            var accountId = AccountId.Load(message.Id);
            var signedValue = subject == nameof(AccountFinancialMovementRemovedDomainEvent)
                ? -message.Value
                : message.Value;

            await ApplyDeltaAsync(cashFlowRepo, webSocketMessage, accountId, message.Date,
                Enumeration<int>.GetByKey<MovementType>(message.Type),
                Enumeration<int>.GetByKey<MovementStatus>(message.Status), signedValue,
                args.CancellationToken);
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    /// <summary>
    /// Applies a signed value (positive to add, negative to reverse) to the CashFlow row for
    /// <paramref name="date"/>'s period, creating the row if needed, then cascades the resulting
    /// balance forward through every later month that already has a row for the account.
    /// </summary>
    private static async Task ApplyDeltaAsync(
        ICommandsCashFlowRepository cashFlowRepo,
        IWebSocketMessage webSocketMessage,
        AccountId accountId,
        DateTime date,
        MovementType type,
        MovementStatus status,
        decimal signedValue,
        CancellationToken cancellationToken)
    {
        var periodKey = (date.Year * 100) + date.Month;

        var current = await cashFlowRepo.GetByAccountAndPeriodAsync(accountId, periodKey, cancellationToken);

        if (current == null)
        {
            var previous = await cashFlowRepo.GetLatestBeforeAsync(accountId, periodKey, cancellationToken);
            var openingBalance = previous?.AccountBalance ?? 0m;

            var (_, created) = CashFlow.New(accountId, Year.Load(date.Year), Month.Load(date.Month), openingBalance);
            current = created!;
        }

        current.ApplyMovement(type, status, signedValue);

        await cashFlowRepo.PersistAsync(current, cancellationToken);

        await webSocketMessage.SendAsync(current.Id, current);

        var runningBalance = current.AccountBalance;
        var following = await cashFlowRepo.GetAfterAsync(accountId, periodKey, cancellationToken);

        foreach (var row in following)
        {
            row.SetFinalBalancePreviousMonth(runningBalance);
            await cashFlowRepo.PersistAsync(row, cancellationToken);
            runningBalance = row.AccountBalance;
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing cash-flow-projection message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Let ExecuteAsync's finally block call StopProcessingAsync on a still-live processor
        // before we dispose it; disposing first races the message pump and disposes its
        // internal SemaphoreSlim out from under it, crashing the host on shutdown.
        await base.StopAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}

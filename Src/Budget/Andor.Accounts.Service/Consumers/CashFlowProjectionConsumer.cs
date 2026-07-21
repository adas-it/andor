using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
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
        if (args.Message.Subject != nameof(AccountFinancialMovementAddedDomainEvent))
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var message = args.Message.Body.ToObjectFromJson<CashFlowMovementMessage>();
        var financialMovementId = FinancialMovementId.Load(message.FinancialMovementId);

        using var scope = _scopeFactory.CreateScope();
        var appliedRepo = scope.ServiceProvider.GetRequiredService<ICashFlowAppliedMovementRepository>();

        if (await appliedRepo.HasBeenAppliedAsync(financialMovementId, args.CancellationToken))
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var cashFlowRepo = scope.ServiceProvider.GetRequiredService<ICommandsCashFlowRepository>();
        var accountId = AccountId.Load(message.Id);
        var periodKey = (message.Date.Year * 100) + message.Date.Month;

        var current = await cashFlowRepo.GetByAccountAndPeriodAsync(accountId, periodKey, args.CancellationToken);

        if (current == null)
        {
            var previous = await cashFlowRepo.GetLatestBeforeAsync(accountId, periodKey, args.CancellationToken);
            var openingBalance = previous?.AccountBalance ?? 0m;

            var (_, created) = CashFlow.New(accountId, Year.Load(message.Date.Year), Month.Load(message.Date.Month), openingBalance);
            current = created!;
        }

        current.ApplyMovement(
            Enumeration<int>.GetByKey<MovementType>(message.Type),
            Enumeration<int>.GetByKey<MovementStatus>(message.Status),
            message.Value);

        await cashFlowRepo.PersistAsync(current, args.CancellationToken);
        await appliedRepo.MarkAppliedAsync(financialMovementId, current.Id, args.CancellationToken);

        var runningBalance = current.AccountBalance;
        var following = await cashFlowRepo.GetAfterAsync(accountId, periodKey, args.CancellationToken);

        foreach (var row in following)
        {
            row.SetFinalBalancePreviousMonth(runningBalance);
            await cashFlowRepo.PersistAsync(row, args.CancellationToken);
            runningBalance = row.AccountBalance;
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing cash-flow-projection message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}

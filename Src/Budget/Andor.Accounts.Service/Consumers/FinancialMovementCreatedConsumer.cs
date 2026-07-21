using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.PaymentMethods.Repositories;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories.Repositories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Domain;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Andor.Accounts.Service.Consumers;

internal sealed record FinancialMovementAddedMessage(
    Guid Id,
    Guid FinancialMovementId,
    DateTime Date,
    string? Description,
    Guid SubCategoryId,
    Guid PaymentMethodId,
    decimal Value,
    int Status);

public sealed class FinancialMovementCreatedSubscriptionOptions
{
    public const string SectionName = "FinancialMovementCreatedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

/// <summary>
/// Consumes the module's "andor-accounts-events" topic and persists the FinancialMovement
/// the Account actor validated but deliberately did not save itself. Reconstructing via
/// FinancialMovement.New(...) with the same id carried in the event makes reprocessing the
/// same message (Service Bus redelivery) an idempotent upsert rather than a duplicate.
/// </summary>
public sealed class FinancialMovementCreatedConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FinancialMovementCreatedConsumer> _logger;

    public FinancialMovementCreatedConsumer(
        ServiceBusClient client,
        IOptions<FinancialMovementCreatedSubscriptionOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<FinancialMovementCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(options.Value.TopicName) || string.IsNullOrWhiteSpace(options.Value.SubscriptionName))
        {
            throw new InvalidOperationException(
                "FinancialMovementCreatedSubscription:TopicName and SubscriptionName must be configured.");
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

        var message = args.Message.Body.ToObjectFromJson<FinancialMovementAddedMessage>();

        using var scope = _scopeFactory.CreateScope();
        var accountRepo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
        var subCategoryRepo = scope.ServiceProvider.GetRequiredService<ICommandsSubCategoryRepository>();
        var paymentMethodRepo = scope.ServiceProvider.GetRequiredService<ICommandsPaymentMethodRepository>();
        var financialMovementRepo = scope.ServiceProvider.GetRequiredService<ICommandsFinancialMovementRepository>();

        var account = await accountRepo.GetByIdAsync(AccountId.Load(message.Id), args.CancellationToken);
        var subCategory = await subCategoryRepo.GetByIdAsync(SubCategoryId.Load(message.SubCategoryId), args.CancellationToken);
        var paymentMethod = await paymentMethodRepo.GetByIdAsync(PaymentMethodId.Load(message.PaymentMethodId), args.CancellationToken);

        if (account == null || subCategory == null || paymentMethod == null)
        {
            _logger.LogError(
                "Cannot persist FinancialMovement {FinancialMovementId}: account, subcategory or payment method not found.",
                message.FinancialMovementId);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var (result, movement) = FinancialMovement.New(
            FinancialMovementId.Load(message.FinancialMovementId),
            message.Date,
            message.Description,
            subCategory,
            paymentMethod,
            account,
            message.Value,
            Enumeration<int>.GetByKey<MovementStatus>(message.Status));

        if (movement != null)
        {
            await financialMovementRepo.PersistAsync(movement, args.CancellationToken);
        }
        else
        {
            _logger.LogError(
                "Failed to reconstruct FinancialMovement {FinancialMovementId}: {Errors}.",
                message.FinancialMovementId,
                string.Join(", ", result.Errors.Select(x => x.Message)));
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing financial-movement-created message.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}

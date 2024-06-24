using Andor.Application.Engagement.Budget.FinancialMovements.Commands;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.DomainEventHandlers;

public class FinancialMovementDomainEventConsumer(IMediator _mediator) :
    IConsumer<FinancialMovementCreatedDomainEvent>,
    IConsumer<FinancialMovementChangedDomainEvent>,
    IConsumer<FinancialMovementDeletedDomainEvent>
{
    public async Task Consume(ConsumeContext<FinancialMovementCreatedDomainEvent> context)
    {
        await _mediator.Send(new FinancialMovementManagedAccountsCommand(context.Message.Current.AccountId));
    }

    public async Task Consume(ConsumeContext<FinancialMovementChangedDomainEvent> context)
    {
        await _mediator.Send(new FinancialMovementManagedAccountsCommand(context.Message.Current.AccountId));
    }

    public async Task Consume(ConsumeContext<FinancialMovementDeletedDomainEvent> context)
    {
        await _mediator.Send(new FinancialMovementManagedAccountsCommand(context.Message.Current.AccountId));
    }
}

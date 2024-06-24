using Andor.Application.Engagement.Budget.FinancialMovements.Commands;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.MonthlyCash.DomainEventHandlers;

public class CashFlowDomainEventConsumer(IMediator _mediator) :
    IConsumer<AccountBalanceChangedDomainEvent>
{
    public async Task Consume(ConsumeContext<AccountBalanceChangedDomainEvent> context)
    {
        await _mediator.Send(new AccountBalanceChangedCashFlowCommand(context.Message));
    }
}

namespace Family.Budget.Application.MonthlyCashFlow.DomainEventsHandler;

using Family.Budget.Application.MonthlyCashFlow.Commands;
using Family.Budget.Domain.Entities.CashFlow.DomainEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal class CashFlowEventsCreatedDomainEventHandler :
    INotificationHandler<CashFlowEventsCreatedDomainEvent>
{
    private readonly IMediator _mediator;

    public CashFlowEventsCreatedDomainEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CashFlowEventsCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateCashFlowToFillGapWithPreviousOneCommand() { Entity = notification.Entity }, cancellationToken);
        await _mediator.Send(new CreateCashFlowToFillGapWithLatestOneCommand() { Entity = notification.Entity }, cancellationToken);
    }
}

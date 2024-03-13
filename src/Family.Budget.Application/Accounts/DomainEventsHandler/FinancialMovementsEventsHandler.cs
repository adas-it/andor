namespace Family.Budget.Application.Accounts.DomainEventsHandler;

using Family.Budget.Application.Accounts.Commands;
using Family.Budget.Domain.Entities.FinancialMovement.DomainEvents;
using MediatR;
using System.Threading.Tasks;

public class FinancialMovementsEventsHandler :
    INotificationHandler<FinancialMovementCreatedDomainEvent>,
    INotificationHandler<FinancialMovementChangedStatusDomainEvent>,
    INotificationHandler<FinancialMovementRemovedDomainEvent>
{
    private readonly IMediator _mediator;

    public FinancialMovementsEventsHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(FinancialMovementCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetMovementsAgesCommandHandlerCommand() { Entity = notification.Entity }, cancellationToken);
    }

    public async Task Handle(FinancialMovementChangedStatusDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetMovementsAgesCommandHandlerCommand() { Entity = notification.Entity }, cancellationToken);
    }

    public async Task Handle(FinancialMovementRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetMovementsAgesCommandHandlerCommand() { Entity = notification.Entity }, cancellationToken);
    }
}
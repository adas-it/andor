namespace Family.Budget.Application.MonthlyCashFlow.DomainEventsHandler;

using Family.Budget.Application.MonthlyCashFlow.Commands;
using Family.Budget.Domain.Entities.CashFlow.DomainEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal class AccountBalanceChangedDomainEventHandler :
    INotificationHandler<AccountBalanceChangedDomainEvent>
{
    private readonly IMediator _mediator;

    public AccountBalanceChangedDomainEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AccountBalanceChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetNewFinalBalanceByChangedAccountBalanceCommand() { Entity = notification.Entity }, cancellationToken);
    }
}

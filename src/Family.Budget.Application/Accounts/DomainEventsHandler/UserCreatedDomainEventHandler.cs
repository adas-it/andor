namespace Family.Budget.Application.Accounts.DomainEventsHandler;

using Family.Budget.Application.Accounts.Commands;
using Family.Budget.Domain.Entities.Users.DomainEvents;
using Hangfire;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IMediator _mediator;

    public UserCreatedDomainEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        BackgroundJob.Enqueue(() => Proceed(notification.Entity.Id,
            notification.Entity.LocationInfos.LocalCurrency.Id,
            notification.Entity.FirstName,
            cancellationToken));

        return Task.CompletedTask;
    }

    public async Task Proceed(Guid userId, Guid currencyId, string firstName, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AccountCommand() { AccountName = $"{firstName},s Accounts", CurrencyId = currencyId, UserId = userId });
    }
}

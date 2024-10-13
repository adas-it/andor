using Andor.Application.Engagement.Budget.Accounts.Commands;
using Andor.Domain.Engagement.Budget.Accounts.Users.DomainEvents;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.DomainEventHandlers;

public class UserDomainEventConsumer(IMediator _mediator) :
    IConsumer<UserCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        var command = new CreateAccountCommand()
        {
            CurrencyId = context.Message.PreferredCurrencyId,
            UserId = context.Message.Id,
            AccountName = $"{context.Message.FirstName}´s Account"
        };

        await _mediator.Send(command);
    }
}

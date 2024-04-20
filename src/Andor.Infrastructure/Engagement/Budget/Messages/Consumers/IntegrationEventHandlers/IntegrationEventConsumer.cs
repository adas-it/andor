using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Application.Engagement.Budget.Accounts.Commands;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.IntegrationEventHandlers;

public class IntegrationEventConsumer(IMediator _mediator) :
    IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var command = new CreateAccountCommand()
        {
            CurrencyId = context.Message.CurrencyId,
            UserId = context.Message.UserId,
            AccountName = context.Message.FirstName + "'s Account"
        };

        await _mediator.Send(command);
    }
}

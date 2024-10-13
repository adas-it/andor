using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Application.Engagement.Budget.Accounts.Commands;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.IntegrationEventHandlers;

public class IntegrationEventConsumer(IMediator _mediator) :
    IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var command = new CreateUserCommand()
        {
            User = context.Message
        };

        await _mediator.Send(command);
    }
}

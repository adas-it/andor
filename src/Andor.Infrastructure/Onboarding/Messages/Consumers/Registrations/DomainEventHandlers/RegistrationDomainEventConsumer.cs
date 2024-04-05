namespace Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations.DomainEventHandlers;
using Andor.Application.Onboarding.Registrations.DomainEventHandlers;
using Andor.Domain.Entities.Onboarding.Registrations.DomainEvents;
using MassTransit;
using MediatR;

public class RegistrationDomainEventConsumer(IMediator _mediator) :
    IConsumer<RegistrationCreatedDomainEvent>,
    IConsumer<RegistrationCodeChangedDomainEvent>
{
    public async Task Consume(ConsumeContext<RegistrationCreatedDomainEvent> context)
    {
        await _mediator.Send(new RequestEmailConfirmationCommand(context.Message.FirstName,
            context.Message.Email,
            context.Message.CheckCode));
    }

    public async Task Consume(ConsumeContext<RegistrationCodeChangedDomainEvent> context)
    {
        await _mediator.Send(new RequestEmailConfirmationCommand(context.Message.FirstName,
            context.Message.Email,
            context.Message.CheckCode));
    }
}

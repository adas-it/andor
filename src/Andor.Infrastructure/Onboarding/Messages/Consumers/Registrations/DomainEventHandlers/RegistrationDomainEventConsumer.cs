namespace Andor.Infrastructure.Onboarding.Messages.Consumers.Registrations.DomainEventHandlers;
using Andor.Application.Onboarding.Registrations.DomainEventHandlers;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using Andor.Domain.Onboarding.Users.DomainEvents;
using MassTransit;
using MediatR;

public class RegistrationDomainEventConsumer(IMediator _mediator) :
    IConsumer<RegistrationCreatedDomainEvent>,
    IConsumer<RegistrationCodeChangedDomainEvent>,
    IConsumer<RegistrationCompletedDomainEvent>,
    IConsumer<UserCreatedDomainEvent>
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

    public Task Consume(ConsumeContext<RegistrationCompletedDomainEvent> context)
    {
        var tasks = new Task[]
        {
            _mediator.Send(new CreateKeycloakUserCommand(context.Message)),
           _mediator.Send(new NotifyRegistrationCompletedCommand(context.Message))
        };

        Task.WaitAll(tasks);

        return Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        await _mediator.Send(new NotifyUserCreatedCommand(context.Message));
    }
}

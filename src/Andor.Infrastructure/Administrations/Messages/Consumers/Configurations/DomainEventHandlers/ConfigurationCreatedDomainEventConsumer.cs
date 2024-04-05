using Andor.Application.Administrations.Configurations.DomainEventHandlers;
using Andor.Domain.Entities.Admin.Configurations.Events;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Administrations.Messaging.Consumers.Configurations.DomainEventHandlers;

public class ConfigurationCreatedDomainEventConsumer(IMediator mediator) : IConsumer<ConfigurationCreatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ConfigurationCreatedDomainEvent> context)
    {
        await _mediator.Send(new ConfigurationCreatedDomainEventCommand(context.Message));
    }
}

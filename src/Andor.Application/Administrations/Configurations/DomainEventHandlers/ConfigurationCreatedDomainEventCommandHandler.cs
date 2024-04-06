namespace Andor.Application.Administrations.Configurations.DomainEventHandlers;

using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Administrations.Configurations.IntegrationsEvents.v1;
using Andor.Domain.Entities.Admin.Configurations.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public record ConfigurationCreatedDomainEventCommand(ConfigurationCreatedDomainEvent message) : IRequest;

public class ConfigurationCreatedDomainEventCommandHandler(IMessageSenderInterface messageSenderInterface) : IRequestHandler<ConfigurationCreatedDomainEventCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(ConfigurationCreatedDomainEventCommand request, CancellationToken cancellationToken)
    {
        await _messageSenderInterface.PubSubSendAsync(new ConfigurationCreated()
        {
            Id = request.message.Id,
        }, cancellationToken);
    }
}

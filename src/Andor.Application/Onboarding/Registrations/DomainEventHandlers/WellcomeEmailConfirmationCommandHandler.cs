using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using Andor.Domain.Administrations.Configurations.Repository;
using MediatR;
using System.Reflection;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record SendWellcomeEmailCommand(string Name, string Email) : IRequest;

public class SendWellcomeEmailCommandHandler(IMessageSenderInterface messageSenderInterface,
    IQueriesConfigurationRepository _configurationRepository)
    : IRequestHandler<SendWellcomeEmailCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(SendWellcomeEmailCommand request, CancellationToken cancellationToken)
    {
        var registrationRule = await _configurationRepository.GetActiveByNameAsync("wellcome_email",
            cancellationToken) ?? throw new InvalidFilterCriteriaException("Configuration not found wellcome_email");

        await _messageSenderInterface.PubSubSendAsync(new RequestCommunication()
        {
            RuleId = Guid.Parse(registrationRule.Value),
            Email = request.Email,
            ContentLanguage = "en",
            Values = new Dictionary<string, string>
            {
                { "<name>", request.Name }
            }
        }, cancellationToken);
    }
}

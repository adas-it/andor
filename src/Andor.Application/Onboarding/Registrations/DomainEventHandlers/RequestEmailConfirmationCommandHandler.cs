using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using MediatR;
using System.Reflection;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record RequestEmailConfirmationCommand(string Name, string Email, string CheckCode) : IRequest;

public class RequestEmailConfirmationCommandHandler(IMessageSenderInterface messageSenderInterface,
    IQueriesConfigurationRepository _configurationRepository)
    : IRequestHandler<RequestEmailConfirmationCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(RequestEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var registrationRule = await _configurationRepository.GetActiveByNameAsync("register_email",
            cancellationToken) ?? throw new InvalidFilterCriteriaException("Configuration not found confirmation_email");

        await _messageSenderInterface.PubSubSendAsync(new RequestCommunication()
        {
            RuleId = Guid.Parse(registrationRule.Value),
            Email = request.Email,
            ContentLanguage = "en",
            Values = new Dictionary<string, string>
            {
                { "<name>", request.Name },
                { "<code>", request.CheckCode }
            }
        }, cancellationToken);
    }
}

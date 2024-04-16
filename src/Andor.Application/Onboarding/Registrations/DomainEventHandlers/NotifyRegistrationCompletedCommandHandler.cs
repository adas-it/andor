using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using MediatR;
using System.Reflection;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record NotifyRegistrationCompletedCommand(RegistrationCompletedDomainEvent message) : IRequest;

public class NotifyRegistrationCompletedCommandHandler(IMessageSenderInterface messageSenderInterface,
    IQueriesConfigurationRepository _configurationRepository)
    : IRequestHandler<NotifyRegistrationCompletedCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(NotifyRegistrationCompletedCommand request, CancellationToken cancellationToken)
    {
        var registrationRule = await _configurationRepository.GetActiveByNameAsync("confirmation_email",
            cancellationToken);

        if (registrationRule is null)
            throw new InvalidFilterCriteriaException("Configuration not found confirmation_email");

        await _messageSenderInterface.PubSubSendAsync(new RegistrationCompleted()
        {
            UserName = request.message.UserName,
            FirstName = request.message.FirstName,
            LastName = request.message.LastName,
            Locale = request.message.Locale,
            Email = request.message.Email,
            AcceptedTermsCondition = request.message.AcceptedTermsCondition,
            AcceptedPrivateData = request.message.AcceptedPrivateData
        }, cancellationToken);
    }
}

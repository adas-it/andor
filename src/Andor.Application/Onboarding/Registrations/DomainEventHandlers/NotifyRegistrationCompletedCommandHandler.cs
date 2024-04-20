using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using MediatR;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record NotifyRegistrationCompletedCommand(RegistrationCompletedDomainEvent message) : IRequest;

public class NotifyRegistrationCompletedCommandHandler(IMessageSenderInterface messageSenderInterface)
    : IRequestHandler<NotifyRegistrationCompletedCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(NotifyRegistrationCompletedCommand request, CancellationToken cancellationToken)
    {
        await _messageSenderInterface.PubSubSendAsync(new RegistrationCompleted()
        {
            CurrencyId = request.message.CurrencyId,
            LanguageId = request.message.LanguageId,
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

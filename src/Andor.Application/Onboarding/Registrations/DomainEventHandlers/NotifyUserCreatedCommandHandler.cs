using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Domain.Onboarding.Users.DomainEvents;
using MediatR;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record NotifyUserCreatedCommand(UserCreatedDomainEvent message) : IRequest;

public class NotifyUserCreatedCommandHandler(IMessageSenderInterface messageSenderInterface)
    : IRequestHandler<NotifyUserCreatedCommand>
{
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;
    public async Task Handle(NotifyUserCreatedCommand request, CancellationToken cancellationToken)
    {
        await _messageSenderInterface.PubSubSendAsync(new UserCreated()
        {
            UserId = request.message.Id,
            CurrencyId = request.message.PreferredCurrencyId,
            LanguageId = request.message.PreferredLanguageId,
            UserName = request.message.UserName,
            FirstName = request.message.FirstName,
            LastName = request.message.LastName,
            Email = request.message.Email,
            AcceptedTermsCondition = request.message.AcceptedTermsCondition,
            AcceptedPrivateData = request.message.AcceptedPrivateData
        }, cancellationToken);
    }
}
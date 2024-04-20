using Andor.Application.Common.Interfaces;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using Andor.Domain.Onboarding.Registrations.Repositories;
using MediatR;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record CreateKeycloakUserCommand(RegistrationCompletedDomainEvent message) : IRequest;

public class CreateKeycloakUserCommandHandler(IKeycloakService _service,
    IQueriesCurrencyRepository queriesCurrencyRepository,
    IQueriesLanguageRepository queriesLanguageRepository,
    IMessageSenderInterface messageSenderInterface)
    : IRequestHandler<CreateKeycloakUserCommand>
{
    public async Task Handle(CreateKeycloakUserCommand request, CancellationToken cancellationToken)
    {
        var currency = await queriesCurrencyRepository.GetByIdAsync(request.message.CurrencyId, cancellationToken);
        var language = await queriesLanguageRepository.GetByIdAsync(request.message.LanguageId, cancellationToken);

        var item = await _service.CreateUser(request.message.Id, request.message.UserName, request.message.FirstName,
            request.message.LastName, request.message.Password, request.message.Email, request.message.Locale, language,
            currency, request.message.AcceptedTermsCondition, request.message.AcceptedPrivateData, cancellationToken);

        item.Events.ToList().ForEach(async x =>
        {
            await messageSenderInterface.PubSubSendAsync(x, cancellationToken);
        });
    }
}

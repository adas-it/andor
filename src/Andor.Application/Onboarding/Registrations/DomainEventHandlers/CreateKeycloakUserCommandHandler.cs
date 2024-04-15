using Andor.Application.Common.Interfaces;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.DomainEventHandlers;

public record CreateKeycloakUserCommand(RegistrationCompletedDomainEvent message) : IRequest;

public class CreateKeycloakUserCommandHandler(IKeycloakService _service)
    : IRequestHandler<CreateKeycloakUserCommand>
{
    public async Task Handle(CreateKeycloakUserCommand request, CancellationToken cancellationToken)
    {
        await _service.CreateUser(request.message.UserName,
            new MailAddress(request.message.Email), request.message.FirstName,
            request.message.LastName, request.message.Password, cancellationToken);
    }
}

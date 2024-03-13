namespace Family.Budget.Application.Registration.DomainEventsHandler;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.Registrations.DomainEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class RegistrationCreatedDomainEventHandler :
    INotificationHandler<RegistrationCreatedDomainEvent>,
    INotificationHandler<RegistrationCodeChangedDomainEvent>
{
    private readonly IRequestRegistrationComunication _requestRegistrationComunication;
    public RegistrationCreatedDomainEventHandler(IRequestRegistrationComunication requestRegistrationComunication)
    {
        _requestRegistrationComunication = requestRegistrationComunication;
    }

    public async Task Handle(RegistrationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _requestRegistrationComunication.Send(notification.Entity, cancellationToken);
    }

    public async Task Handle(RegistrationCodeChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _requestRegistrationComunication.Send(notification.Entity, cancellationToken);
    }
}

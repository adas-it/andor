namespace Family.Budget.Application.@Common.Interfaces;
using Family.Budget.Domain.Entities.Registrations;

public interface IRequestRegistrationComunication
{
    Task Send(Registration registration, CancellationToken cancellationToken);
}

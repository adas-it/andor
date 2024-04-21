using Andor.Domain.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface ICommandsRegistrationRepository : ICommandRepository<Registration, RegistrationId>
{
}

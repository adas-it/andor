using Andor.Domain.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface ICommandsRegistrationRepository : ICommandRepository<Registration, RegistrationId>
{
}

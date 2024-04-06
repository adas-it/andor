using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Entities.Onboarding.Registrations.Repositories;

public interface ICommandsRegistrationRepository : ICommandRepository<Registration, RegistrationId>
{
}

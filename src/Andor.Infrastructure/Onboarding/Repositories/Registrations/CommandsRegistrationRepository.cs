using Andor.Domain.Entities.Onboarding.Registrations;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Onboarding.Repositories.Registrations;

public class CommandsRegistrationRepository(PrincipalContext context) :
    CommandsBaseRepository<Registration, RegistrationId>(context),
    ICommandsRegistrationRepository
{
}

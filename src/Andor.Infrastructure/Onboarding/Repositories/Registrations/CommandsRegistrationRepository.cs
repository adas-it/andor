using Andor.Domain.Onboarding.Registrations;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Domain.Onboarding.Registrations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Onboarding.Repositories.Registrations;

public class CommandsRegistrationRepository(PrincipalContext context) :
    CommandsBaseRepository<Registration, RegistrationId>(context),
    ICommandsRegistrationRepository
{
}

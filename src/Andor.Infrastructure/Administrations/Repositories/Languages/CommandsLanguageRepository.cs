using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.Communications.Repositories;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Administrations.Repositories.Configurations;

public class CommandsLanguageRepository(PrincipalContext context) :
    CommandsBaseRepository<Language, LanguageId>(context),
    ICommandsLanguageRepository
{
}

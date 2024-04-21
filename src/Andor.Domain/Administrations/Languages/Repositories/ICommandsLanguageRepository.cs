using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Administrations.Languages.Repositories;

public interface ICommandsLanguageRepository : ICommandRepository<Language, LanguageId>
{
}
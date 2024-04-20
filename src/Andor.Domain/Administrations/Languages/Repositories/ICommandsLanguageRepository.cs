using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Communications.Repositories;

public interface ICommandsLanguageRepository : ICommandRepository<Language, LanguageId>
{
}
using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Administrations.Languages.Repositories;

public interface IQueriesLanguageRepository :
    IResearchableRepository<Language, LanguageId, SearchInput>
{
}
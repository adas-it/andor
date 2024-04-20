using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Communications.Repositories;

public interface IQueriesLanguageRepository :
    IResearchableRepository<Language, LanguageId, SearchInput>
{
}
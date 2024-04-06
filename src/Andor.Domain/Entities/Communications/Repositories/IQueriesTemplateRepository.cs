using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Entities.Communications.Repositories;

public interface IQueriesTemplateRepository :
    IResearchableRepository<Template, TemplateId, SearchInput>
{
}
using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Communications.Repositories;

public interface IQueriesTemplateRepository :
    IResearchableRepository<Template, TemplateId, SearchInput>
{
}
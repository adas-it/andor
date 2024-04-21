using Andor.Domain.Communications;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Communications.Repositories;

public interface IQueriesTemplateRepository :
    IResearchableRepository<Template, TemplateId, SearchInput>
{
}
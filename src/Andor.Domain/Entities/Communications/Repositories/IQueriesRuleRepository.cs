using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Entities.Communications.Repositories;

public interface IQueriesRuleRepository :
    IResearchableRepository<Rule, RuleId, SearchInput>
{
}
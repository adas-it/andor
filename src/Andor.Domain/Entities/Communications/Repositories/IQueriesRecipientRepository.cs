using Andor.Domain.Entities.Communications.Users;
using Andor.Domain.Entities.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Entities.Communications.Repositories;

public interface IQueriesRecipientRepository :
    IResearchableRepository<Recipient, RecipientId, SearchInput>
{
}
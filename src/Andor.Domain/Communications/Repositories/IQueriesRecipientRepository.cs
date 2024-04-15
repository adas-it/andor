using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;

namespace Andor.Domain.Communications.Repositories;

public interface IQueriesRecipientRepository :
    IResearchableRepository<Recipient, RecipientId, SearchInput>
{
}
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories
{
    public interface IQueriesInviteRepository :
    IResearchableRepository<Invite, InviteId, SearchInputInvite>
    {
        Task<SearchOutput<Invite>> SearchPendingsInvitesAsync(SearchInput input, CancellationToken cancellationToken);
    }
}

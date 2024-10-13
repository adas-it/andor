using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories
{
    public interface ICommandsInviteRepository : ICommandRepository<Invite, InviteId>
    {
        public Task<List<Invite>> GetAllPendingByGuestEmailAsync(string mail, CancellationToken cancellationToken);
    }
}

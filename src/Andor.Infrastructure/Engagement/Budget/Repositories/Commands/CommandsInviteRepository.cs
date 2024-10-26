using Andor.Domain.Engagement.Budget.Accounts.Invites;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Commands
{
    public class CommandsInviteRepository(PrincipalContext context) :
    CommandsBaseRepository<Invite, InviteId>(context), ICommandsInviteRepository
    {

        public override async Task<Invite?> GetByIdAsync(InviteId id, CancellationToken cancellationToken)
        => await _dbSet
            .Include(x => x.Inviting)
            .Include(x => x.Guest)
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        public async Task<List<Invite>> GetAllPendingByGuestEmailAsync(string mail, CancellationToken cancellationToken)
        {
            var invites = await _dbSet.Where(x => x.Email == mail).ToListAsync(cancellationToken);

            invites = invites.Where(z => z.Status == InviteStatus.Pending).ToList();
            return invites;
        }
    }
}

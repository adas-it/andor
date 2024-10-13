using Andor.Domain.Engagement.Budget.Accounts.Users;
using Andor.Domain.Engagement.Budget.Accounts.Users.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Commands
{
    public class CommandsUserRepository(PrincipalContext context) :
    CommandsBaseRepository<User, UserId>(context), ICommandsUserRepository
    {
        public async Task<User?> GetByMailAsync(MailAddress email, CancellationToken cancellationToken)
        => await _dbSet
            .FirstOrDefaultAsync(x => x.Email.Equals(email), cancellationToken);
    }
}

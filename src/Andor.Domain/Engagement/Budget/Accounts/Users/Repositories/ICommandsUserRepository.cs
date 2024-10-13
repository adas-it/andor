using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;
using System.Net.Mail;

namespace Andor.Domain.Engagement.Budget.Accounts.Users.Repositories;

public interface ICommandsUserRepository : ICommandRepository<User, UserId>
{
    Task<User?> GetByMailAsync(MailAddress email, CancellationToken cancellationToken);
}

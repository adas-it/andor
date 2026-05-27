using Andor.Foundation.Domain.SeedWork.CommandRepository;
using System.Net.Mail;
using Users.Users.ValueObjects;

namespace Users.Users.Repositories;

public interface ICommandsUserRepository : ICommandRepository<User, UserId>
{
    Task<User?> GetByMailAsync(MailAddress email, CancellationToken cancellationToken);
}

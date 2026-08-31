using Andor.Foundation.Domain.SeedWork.CommandRepository;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Domain.Users.Repositories;

public interface ICommandsUserRepository : ICommandRepository<User, UserId>
{
    Task<User?> GetByMailAsync(Email email, CancellationToken cancellationToken);
}

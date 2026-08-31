using Andor.Users.Domain.Users;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application.Interfaces;

public interface IQueriesUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);
}

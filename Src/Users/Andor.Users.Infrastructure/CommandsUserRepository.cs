using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users;
using Andor.Users.Domain.Users.Repositories;
using Andor.Users.Domain.Users.ValueObjects;
using Andor.Users.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Users.Infrastructure;

public class CommandsUserRepository(UserContext context) : ICommandsUserRepository
{
    protected readonly DbSet<User> DbSet = context.Set<User>();

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken)
        => await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<User?> GetByMailAsync(Email email, CancellationToken cancellationToken)
    {
        var address = email.Value;
        return await DbSet.FirstOrDefaultAsync(x => x.Email == address, cancellationToken);
    }

    public async Task PersistAsync(User entity, CancellationToken cancellationToken)
    {
        context.Upsert<User, UserId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

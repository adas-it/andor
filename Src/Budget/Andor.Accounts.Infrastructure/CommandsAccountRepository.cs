using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsAccountRepository(AccountsContext context) : ICommandsAccountRepository
{
    protected readonly DbSet<Account> DbSet = context.Set<Account>();

    public Task<Account?> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Include(x => x.Categories)
            .Include(x => x.SubCategories)
            .Include(x => x.PaymentMethods)
            .Include(x => x.Members)
            .Include(x => x.Invites)
            .Include(x => x.Currency)
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult<Account?>(entity);
    }

    public async Task PersistAsync(Account entity, CancellationToken cancellationToken)
    {
        context.Upsert<Account, AccountId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

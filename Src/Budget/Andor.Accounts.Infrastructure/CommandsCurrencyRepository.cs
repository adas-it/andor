using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsCurrencyRepository(AccountsContext context) : ICommandsCurrencyRepository
{
    protected readonly DbSet<Currency> DbSet = context.Set<Currency>();

    public Task<Currency?> GetByIdAsync(CurrencyId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<Currency?> GetByIsoAsync(string iso, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Iso == iso);

        return Task.FromResult(entity);
    }

    public async Task PersistAsync(Currency entity, CancellationToken cancellationToken)
    {
        context.Upsert<Currency, CurrencyId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

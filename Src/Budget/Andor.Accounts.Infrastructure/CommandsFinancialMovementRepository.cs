using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsFinancialMovementRepository(AccountsContext context) : ICommandsFinancialMovementRepository
{
    protected readonly DbSet<FinancialMovement> DbSet = context.Set<FinancialMovement>();

    public Task<FinancialMovement?> GetByIdAsync(FinancialMovementId id, CancellationToken cancellationToken)
    {
        var entity = DbSet
            .Include(x => x.SubCategory)
            .Include(x => x.PaymentMethod)
            .Include(x => x.Account)
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public async Task PersistAsync(FinancialMovement entity, CancellationToken cancellationToken)
    {
        context.Upsert<FinancialMovement, FinancialMovementId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

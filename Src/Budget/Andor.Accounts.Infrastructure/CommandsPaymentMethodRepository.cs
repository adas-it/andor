using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.Repositories;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class CommandsPaymentMethodRepository(AccountsContext context) : ICommandsPaymentMethodRepository
{
    protected readonly DbSet<PaymentMethod> DbSet = context.Set<PaymentMethod>();

    public Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<PaymentMethod>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<PaymentMethod> entities = DbSet.Where(x => x.Owner == null).ToList();

        return Task.FromResult(entities);
    }

    public async Task PersistAsync(PaymentMethod entity, CancellationToken cancellationToken)
    {
        context.Upsert<PaymentMethod, PaymentMethodId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

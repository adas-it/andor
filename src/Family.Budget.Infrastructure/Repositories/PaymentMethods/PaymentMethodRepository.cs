namespace Family.Budget.Infrastructure.Repositories.PaymentMethod;

using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class PaymentMethodRepository : QueryHelper<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(PaymentMethod entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(PaymentMethod entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(PaymentMethod entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var item = await _dbSet.FindAsync(ids, cancellationToken);

        if (item != null)
        {
            _dbSet.Remove(item);
        }
    }

    public async Task<PaymentMethod?> GetById(Guid id, CancellationToken cancellationToken)
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<PaymentMethod>> Search(SearchInput input, MovementType type, CancellationToken cancellationToken)
    {
        Expression<Func<PaymentMethod, bool>> where = x => x.Type.Equals(type);

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower()) && x.Type.Equals(type);

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<PaymentMethod>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<PaymentMethod>> GetByName(string name, CancellationToken cancellationToken)
    {
        var ret = new List<PaymentMethod>();

        var items = GetMany(x => x.Name.Equals(name))
            .ToList();

        if (items.Any()) ret.AddRange(items!);

        return Task.FromResult(ret);
    }

    public async Task<List<PaymentMethod>> GetByIds(List<Guid> ids, CancellationToken cancellationToken)
        => await _dbSet.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
}

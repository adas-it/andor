namespace Family.Budget.Infrastructure.Repositories.Registrations;

using Family.Budget.Domain.Entities.Registrations;
using Family.Budget.Domain.Entities.Registrations.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class RegistrationsRepository : QueryHelper<Registration>, IRegistrationRepository
{
    public RegistrationsRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(Registration entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(Registration entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Registration entity, CancellationToken cancellationToken)
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

    public async Task<Registration?> GetById(Guid id, CancellationToken cancellationToken)
        => await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Registration>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Registration, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.FirstName.ToLower().Contains(input.Search.ToLower());

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Registration>(input.Page, input.PerPage, total, items!));
    }

    public async Task<Registration?> GetByEmail(string email, CancellationToken cancellationToken)
        => await _dbSet
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<List<Registration>> GetOldRegistrations(DateTimeOffset dateTimeOffset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

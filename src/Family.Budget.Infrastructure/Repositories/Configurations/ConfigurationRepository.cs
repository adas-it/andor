namespace Family.Budget.Infrastructure.Repositories.Configurations;

using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class ConfigurationRepository : QueryHelper<Configuration>, IConfigurationRepository
{
    public ConfigurationRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(Configuration entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(Configuration entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Configuration entity, CancellationToken cancellationToken)
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

    public async Task<Configuration?> GetById(Guid id, CancellationToken cancellationToken)
    => await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Configuration>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Configuration, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower());

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Configuration>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<Configuration>> GetByName(string name, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name)).AsNoTracking().ToList());

    public async Task<Configuration?> GetByNameActive(string name, CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(name) && 
        x.StartDate <= DateTime.UtcNow && 
        (x.FinalDate == null || (x.FinalDate != null && x.FinalDate >= DateTime.UtcNow)));
}

namespace Andor.Infrastructure.Repositories.Common;
using Andor.Domain.SeedWork;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

public class QueryHelper<TEntity, TEntityId>(PrincipalContext context) 
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken)
    => await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    protected virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        => _dbSet.AsNoTracking().Where(where);

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => GetManyPaginated(where, orderBy, order, page, perPage, null!, out totalPages);

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        Expression<Func<TEntity, object>> include,
        out int totalPages)
    {
        var query = _dbSet.AsNoTracking();

        if (where != null)
        {
            query = query.Where(where);
        }

        totalPages = query.Count();

        if (string.IsNullOrEmpty(orderBy) is false)
        {
            var field = typeof(TEntity).GetProperties()
                .AsEnumerable()
                .FirstOrDefault(x => x.Name.ToLower()
                .Equals(orderBy.ToLower()));

            if (field != null)
            {
                if (order == SearchOrder.Asc)
                    query = query.OrderBy(ToLambda<TEntity>(field.Name));

                if (order == SearchOrder.Desc)
                    query = query.OrderByDescending(ToLambda<TEntity>(field.Name));
            }
        }
        else
        {
            query = query.OrderBy(z => z.Id);
        }

        if (include != null)
        {
            query = query.Include(include);
        }

        return query.Skip(page * perPage).Take(perPage);
    }

    protected virtual IQueryable<TOutput> GetManyPaginated<TOutput>(Func<IQueryable<TOutput>>? where,
        Dictionary<string, SearchOrder>? orderBy,
        Expression<Func<TEntity, TOutput>> project,
        int? page,
        int? perPage,
        out int total)
    {
        var query = _dbSet.AsNoTracking().Select(project);

        if (where != null)
        {
            query = where.Invoke();
        }

        total = query.Count();

        orderBy ??= [];

        foreach (var item in orderBy)
        {
            var field = typeof(TOutput).GetProperties()
                .AsEnumerable()
                .FirstOrDefault(x => x.Name.Equals(item.Key, StringComparison.InvariantCulture));

            if (field != null)
            {
                if (item.Value == SearchOrder.Asc)
                    query = ((IOrderedQueryable<TOutput>)query).ThenBy(ToLambda<TOutput>(field.Name));

                if (item.Value == SearchOrder.Desc)
                    query = ((IOrderedQueryable<TOutput>)query).ThenByDescending(ToLambda<TOutput>(field.Name));
            }
        }

        if(page.HasValue && perPage.HasValue)
        {
            return query.Skip(page.Value * perPage.Value).Take(perPage.Value);
        }

        return query;
    }

    private static Expression<Func<TProp, object>> ToLambda<TProp>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TProp, object>>(propAsObject, parameter);
    }
}


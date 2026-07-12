using System.Linq.Expressions;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure;

public class QueryHelper<TEntity, TEntityId>(DbContext context)
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();
    protected Expression<Func<TEntity, bool>>? loggedUserFilter;

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken)
    => await DbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    protected virtual IQueryable<TEntity> GetMany(List<Expression<Func<TEntity, bool>>> where)
    {
        var query = DbSet.AsNoTracking();

        if (loggedUserFilter is not null)
        {
            query = query.Where(loggedUserFilter);
        }

        if (where != null && where.Count != 0)
        {
            query = where.Aggregate(query, (current, item) => current.Where(item));
        }

        return query;
    }

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => Extension.GetManyPaginated(
            DbSet, loggedUserFilter, [where], orderBy, order, page, perPage, null!, out totalPages);

    protected virtual IQueryable<TEntity> GetManyPaginated(List<Expression<Func<TEntity, bool>>>? where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => Extension.GetManyPaginated(
            DbSet, loggedUserFilter, where, orderBy, order, page, perPage, null!, out totalPages);
}

public static class Extension
{
    public static IQueryable<TEntity> GetManyPaginated<TDbSet, TEntity>(
        TDbSet dbSet,
        Expression<Func<TEntity, bool>>? loggedUserFilter,
        List<Expression<Func<TEntity, bool>>>? where,
        string? orderBy, SearchOrder order, int page, int perPage, out int totalPages)
        where TDbSet : DbSet<TEntity>
        where TEntity : class
    {
        return GetManyPaginated<DbSet<TEntity>, TEntity>(
            dbSet, loggedUserFilter, where, orderBy, order, page, perPage, null!, out totalPages);

    }

    public static IQueryable<TEntity> GetManyPaginated<TDbSet, TEntity>(
        TDbSet dbSet,
        Expression<Func<TEntity, bool>>? loggedUserFilter,
        List<Expression<Func<TEntity, bool>>>? where,
        string? orderBy, SearchOrder order, int page, int perPage,
        Expression<Func<TEntity, object>>? include, out int totalPages)
        where TDbSet : DbSet<TEntity>
        where TEntity : class
    {
        var query = dbSet.AsNoTracking();

        if (loggedUserFilter is not null)
        {
            query = query.Where(loggedUserFilter);
        }

        if (where != null)
        {
            query = where.Aggregate(query, (current, item) => current.Where(item));
        }

        totalPages = query.Count();

        if (!string.IsNullOrEmpty(orderBy))
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

        if (include != null)
        {
            query = query.Include(include);
        }

        return query.Skip(page * perPage).Take(perPage);
    }

    public static IQueryable<TOutput> GetManyPaginated<TOutput, TDbSet, TEntity>(
        TDbSet dbSet,
        Expression<Func<TEntity, bool>>? loggedUserFilter,
        List<Expression<Func<TEntity, bool>>>? where,
        Dictionary<string, SearchOrder>? orderBy,
        Expression<Func<TEntity, TOutput>> project,
        int? page,
        int? perPage,
        out int total)
        where TDbSet : DbSet<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> query;

        query = loggedUserFilter is not null ? dbSet.AsNoTracking().Where(loggedUserFilter) : dbSet.AsNoTracking();

        if (where != null && where.Count != 0)
        {
            query = where.Aggregate(query, (current, item) => current.Where(item));
        }

        var queryProjected = query.Select(project);

        total = queryProjected.Count();

        orderBy ??= [];

        foreach (var item in orderBy)
        {
            var field = typeof(TOutput).GetProperties()
                .AsEnumerable()
                .FirstOrDefault(x => x.Name.Equals(item.Key, StringComparison.InvariantCulture));

            if (field != null)
            {
                if (item.Value == SearchOrder.Asc)
                    queryProjected = ((IOrderedQueryable<TOutput>)queryProjected).ThenBy(ToLambda<TOutput>(field.Name));

                if (item.Value == SearchOrder.Desc)
                    queryProjected = ((IOrderedQueryable<TOutput>)queryProjected).ThenByDescending(ToLambda<TOutput>(field.Name));
            }
        }

        if (page.HasValue && perPage.HasValue)
        {
            return queryProjected.Skip(page.Value * perPage.Value).Take(perPage.Value);
        }

        return queryProjected;
    }

    private static Expression<Func<TProp, object>> ToLambda<TProp>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TProp));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TProp, object>>(propAsObject, parameter);
    }
}

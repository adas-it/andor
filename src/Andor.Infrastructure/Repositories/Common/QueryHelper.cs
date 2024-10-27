namespace Andor.Infrastructure.Repositories.Common;

using Andor.Domain.SeedWork;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

public class QueryHelper<TEntity, TEntityId>(PrincipalContext context)
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    protected Expression<Func<TEntity, bool>>? loggedUserFilter;

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken)
    => await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    protected virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
    {
        var query = _dbSet.AsNoTracking();

        if (loggedUserFilter is not null)
        {
            query = query.Where(loggedUserFilter);
        }

        return query.Where(where);
    }

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => Extension.GetManyPaginated(
            _dbSet, loggedUserFilter, new List<Expression<Func<TEntity, bool>>>() { where }, orderBy, order, page, perPage, null!, out totalPages);

    protected virtual IQueryable<TEntity> GetManyPaginated(List<Expression<Func<TEntity, bool>>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => Extension.GetManyPaginated(
            _dbSet, loggedUserFilter, where, orderBy, order, page, perPage, null!, out totalPages);
}

public static class Extension
{
    public static IQueryable<TEntity> GetManyPaginated<TDbSet, TEntity>(
        TDbSet _dbSet,
        Expression<Func<TEntity, bool>>? loggedUserFilter,
        List<Expression<Func<TEntity, bool>>> where,
        string? orderBy, SearchOrder order, int page, int perPage, out int totalPages)
        where TDbSet : DbSet<TEntity>
        where TEntity : class
    {
        return GetManyPaginated<DbSet<TEntity>, TEntity>(
            _dbSet, loggedUserFilter, where, orderBy, order, page, perPage, null!, out totalPages);

    }

    public static IQueryable<TEntity> GetManyPaginated<TDbSet, TEntity>(
        TDbSet _dbSet,
        Expression<Func<TEntity, bool>>? loggedUserFilter,
        List<Expression<Func<TEntity, bool>>> where,
        string? orderBy, SearchOrder order, int page, int perPage,
        Expression<Func<TEntity, object>> include, out int totalPages)
        where TDbSet : DbSet<TEntity>
        where TEntity : class
    {
        var query = _dbSet.AsNoTracking();

        if (loggedUserFilter is not null)
        {
            query = query.Where(loggedUserFilter);
        }

        if (where != null)
        {
            foreach (var item in where)
            {
                query = query.Where(item);
            }
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
        TDbSet _dbSet,
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

        if (loggedUserFilter is not null)
        {
            query = _dbSet.AsNoTracking().Where(loggedUserFilter);
        }
        else
        {
            query = _dbSet.AsNoTracking();
        }

        if (where != null && where.Any())
        {
            foreach (var item in where)
            {
                query = query.Where(item);
            }
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

    public static Expression<Func<TProp, object>> ToLambda<TProp>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TProp));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TProp, object>>(propAsObject, parameter);
    }
}


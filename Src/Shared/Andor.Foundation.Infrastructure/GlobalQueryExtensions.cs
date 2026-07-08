using System.Linq.Expressions;
using System.Reflection;
using Andor.Foundation.Domain;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Andor.Foundation.Infrastructure;

public static class GlobalQueryExtensions
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        var method = typeof(GlobalQueryExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(entityType.ClrType);

        if (method == null)
            return;

        var filter = method.Invoke(null, []);
        entityType.SetQueryFilter((LambdaExpression)filter!);
        entityType.AddIndex(entityType.FindProperty(nameof(ISoftDeletableEntity.IsDeleted))!);
    }

    private static Expression<Func<TEntity, bool>> GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletableEntity
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}

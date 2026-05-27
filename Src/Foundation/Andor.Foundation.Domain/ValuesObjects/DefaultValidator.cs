using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;

namespace Andor.Foundation.Domain.ValuesObjects;

public class DefaultValidator<TEntity, TEntityId> : IDefaultValidator<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{

    public virtual async Task<List<Notification>> ValidateCreationAsync(TEntity entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        await DefaultValidationsAsync(entity, notifications, cancellationToken);

        return notifications;
    }

    protected virtual Task DefaultValidationsAsync(
        TEntity entity,
        List<Notification> notifications,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        _ = notifications ?? throw new ArgumentNullException(nameof(notifications));

        return Task.CompletedTask;
    }

    protected static void AddNotification(Notification? notification, List<Notification> list)
    {
        if (notification != null)
        {
            list.Add(notification);
        }
    }
}

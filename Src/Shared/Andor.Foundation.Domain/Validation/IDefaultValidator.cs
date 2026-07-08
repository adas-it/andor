using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.Validation;

public interface IDefaultValidator<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{
    Task<List<Notification>> ValidateCreationAsync(TEntity entity,
        CancellationToken cancellationToken);
}

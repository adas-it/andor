using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.SeedWork.CommandRepository;

public interface ICommandRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{
    Task PersistAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken);
}

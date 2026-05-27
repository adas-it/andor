using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.SeedWork.CommandRepository;

public interface ICommandRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntityId id, CancellationToken cancellationToken);

    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken);
}

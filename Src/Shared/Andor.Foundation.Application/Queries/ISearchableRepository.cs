namespace Andor.Foundation.Application.Queries;

public interface ISearchableRepository<TEntity, TEntityId, TSearchInput>
{
    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken);

    Task<SearchOutput<TEntity>> SearchAsync(TSearchInput input, CancellationToken cancellationToken);
}

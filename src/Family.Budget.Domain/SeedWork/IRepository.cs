namespace Family.Budget.Domain.SeedWork;
public interface IRepository<T> where T : Entity
{
    Task Insert(T entity, CancellationToken cancellationToken);
    Task Update(T entity, CancellationToken cancellationToken);

    Task Delete(T entity, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);

    Task<T?> GetById(Guid id, CancellationToken cancellationToken);
}
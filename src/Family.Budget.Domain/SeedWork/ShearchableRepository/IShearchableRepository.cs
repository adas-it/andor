namespace Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IShearchableRepository<T> where T : Entity
{
    Task<SearchOutput<T>> Search(SearchInput input, CancellationToken cancellationToken);
}

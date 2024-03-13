namespace Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface ISearchableRepository<T, R> where T : Entity where R : SearchInput
{
    Task<SearchOutput<T>> Search(R input, CancellationToken cancellationToken);
}

namespace Family.Budget.Domain.Entities.Users.Repository;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IUserRepository : IRepository<User>, ISearchableRepository<User, SearchInput>
{
}

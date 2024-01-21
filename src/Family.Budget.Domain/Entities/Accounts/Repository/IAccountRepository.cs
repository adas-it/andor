namespace Family.Budget.Domain.Entities.Accounts.Repository;

using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IAccountRepository : IRepository<Account>, ISearchableRepository<Account, SearchInputAccount>
{
    Task<Account?> GetByIdandUser(AccountId AccountId, UserId UserId, CancellationToken cancellationToken);
}

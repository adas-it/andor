using Family.Budget.Domain.Entities.Users;

namespace Family.Budget.Domain.Entities.Accounts;
public class AccountUser
{
    public AccountUser()
    { }

    public AccountUser(Account account, UserId userId)
    {
        Account = account;
        UserId = userId;
    }

    public Guid AccountId { get; set; }
    public Account Account { get; set; }
    public Guid UserId { get; set; }
}

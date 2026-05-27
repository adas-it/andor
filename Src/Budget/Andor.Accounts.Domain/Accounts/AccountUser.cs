using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users;

namespace Andor.Accounts.Domain.Accounts;

public class AccountUser
{
    public AccountUser()
    {

    }

    public AccountUser(Account account, User user, PermissionType permissionType, int order)
    {
        AccountId = account.Id;
        Account = account;
        UserId = user.Id;
        User = user;
        PermissionType = permissionType;
        Order = order;
    }

    public AccountId AccountId { get; set; }
    public Account Account { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public PermissionType PermissionType { get; set; }
    public int Order { get; set; }
}

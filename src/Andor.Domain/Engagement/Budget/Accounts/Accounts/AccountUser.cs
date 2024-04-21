using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Onboarding.Users.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts;

public class AccountUser
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public UserId UserId { get; set; }
}

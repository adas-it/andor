using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts;

public class AccountCategory
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public CategoryId CategoryId { get; set; }
    public Category? Category { get; set; }
    public int Order { get; set; }
}

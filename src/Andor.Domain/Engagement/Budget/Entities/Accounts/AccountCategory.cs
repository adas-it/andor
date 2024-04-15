using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.Categories;
using Andor.Domain.Engagement.Budget.Entities.Categories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts;

public class AccountCategory
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public CategoryId CategoryId { get; set; }
    public Category? Category { get; set; }
}

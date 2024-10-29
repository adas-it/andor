using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts;

public class AccountSubCategory
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public SubCategoryId SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }
    public int Order { get; set; }
}

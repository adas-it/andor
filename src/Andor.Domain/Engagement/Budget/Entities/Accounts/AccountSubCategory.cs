using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.SubCategories;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts;

public class AccountSubCategory
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public SubCategoryId SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }
}

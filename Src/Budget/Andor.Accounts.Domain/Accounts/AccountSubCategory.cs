using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;

namespace Andor.Accounts.Domain.Accounts;

public class AccountSubCategory
{
    public AccountSubCategory()
    {
    }

    public AccountSubCategory(Account account, SubCategory subCategory, int order)
    {
        AccountId = account.Id;
        Account = account;
        SubCategoryId = subCategory.Id;
        SubCategory = subCategory;
        Order = order;
    }

    public AccountId AccountId { get; set; }
    public Account Account { get; set; }
    public SubCategoryId SubCategoryId { get; set; }
    public SubCategory SubCategory { get; set; }
    public int Order { get; set; }
}

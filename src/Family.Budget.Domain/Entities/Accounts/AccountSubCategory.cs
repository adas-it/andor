namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Entities.SubCategories;

public class AccountSubCategory
{
    public AccountSubCategory()
    { }

    public AccountSubCategory(Account account, SubCategory subCategory)
    {
        Account = account;
        SubCategory = subCategory;
    }

    public Guid AccountId { get; set; }
    public Account Account { get; set; }
    public Guid SubCategoryId { get; set; }
    public SubCategory SubCategory { get; set; }
}

namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Entities.Categories;

public class AccountCategory
{
    public AccountCategory()
    {

    }

    public AccountCategory(Account account, Category category)
    {
        Account = account;
        Category = category;
    }
    public Guid AccountId { get; set; }
    public Account Account { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}

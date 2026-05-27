using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Categories.ValueObjects;

namespace Andor.Accounts.Domain.Accounts;

public class AccountCategory
{
    public AccountCategory()
    {
    }

    public AccountCategory(Account account, Category category, int order)
    {
        AccountId = account.Id;
        Account = account;
        CategoryId = category.Id;
        Category = category;
        Order = order;
    }

    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public CategoryId CategoryId { get; set; }
    public Category? Category { get; set; }
    public int Order { get; set; }
}

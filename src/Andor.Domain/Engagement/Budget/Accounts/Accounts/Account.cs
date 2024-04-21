using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Invites;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts;

public class Account : AggregateRoot<AccountId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Currency? Currency { get; private set; }
    public bool Deleted { get; private set; }
    public DateTime? FirstMovement { get; private set; }
    public DateTime? LastMovement { get; private set; }

    public ICollection<AccountCategory> Categories { get; set; }
    public ICollection<AccountSubCategory> SubCategories { get; set; }
    public ICollection<AccountPaymentMethod> PaymentMethods { get; set; }
    public ICollection<AccountUser> Users { get; set; }
    public ICollection<Invite> Invites { get; set; }

    private Account()
    {
        Id = AccountId.New();
        Name = string.Empty;
        Description = string.Empty;
        Categories = [];
        SubCategories = [];
        PaymentMethods = [];
        Users = [];
        Invites = [];
    }

    private DomainResult SetValues(AccountId id,
        string name,
        string description,
        Currency? currency,
        bool deleted,
        DateTime? firstMovement,
        DateTime? lastMovement)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Name = name;
        Description = description;
        Currency = currency;
        Deleted = deleted;
        FirstMovement = firstMovement;
        LastMovement = lastMovement;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Account?) New(string name,
        string description,
        ICollection<Category> categories,
        ICollection<SubCategory> subCategories,
        ICollection<PaymentMethod> paymentMethods,
        ICollection<Guid> users,
        Currency currency)
    {
        var entity = new Account();

        var result = entity.SetValues(AccountId.New(), name, description, currency, false, null, null);

        categories.ToList().ForEach(x => entity.Categories
        .Add(new AccountCategory()
        {
            Account = entity,
            AccountId = entity.Id,
            Category = x,
            CategoryId = x.Id
        }));

        subCategories.ToList().ForEach(x => entity.SubCategories
        .Add(new AccountSubCategory()
        {
            Account = entity,
            AccountId = entity.Id,
            SubCategory = x,
            SubCategoryId = x.Id
        }));

        paymentMethods.ToList().ForEach(x => entity.PaymentMethods
        .Add(new AccountPaymentMethod()
        {
            Account = entity,
            AccountId = entity.Id,
            PaymentMethod = x,
            PaymentMethodId = x.Id
        }));

        users.ToList().ForEach(x => entity.Users
        .Add(new AccountUser()
        {
            Account = entity,
            AccountId = entity.Id,
            UserId = x
        }));

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(AccountCreatedDomainEvent.FromAggregator(entity));

        return (result, entity);
    }
}

using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Engagement.Budget.Entities.Invites;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts;

public class Account : AggregateRoot<AccountId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Currency? Currency { get; private set; }
    public bool Deleted { get; private set; }
    public DateTime FirstMovement { get; private set; }
    public DateTime LastMovement { get; private set; }

    public ICollection<AccountCategory> Categories { get; private set; }
    public ICollection<AccountSubCategory> SubCategories { get; private set; }
    public ICollection<AccountPaymentMethod> PaymentMethods { get; private set; }
    public ICollection<AccountUser> UserIds { get; private set; }
    public ICollection<Invite> Invites { get; private set; }

    private Account()
    {
        Id = AccountId.New();
        Name = string.Empty;
        Description = string.Empty;
        Categories = [];
        SubCategories = [];
        PaymentMethods = [];
        UserIds = [];
        Invites = [];
    }

    private DomainResult SetValues(AccountId id,
        string name)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Name = name;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Account?) New(
        string name)
    {
        var entity = new Account();

        var result = entity.SetValues(AccountId.New(), name);

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(AccountCreatedDomainEvent.FromAggregator(entity));

        return (result, entity);
    }
}

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

    public IReadOnlyCollection<AccountCategory> Categories => [.. _categories];
    private ICollection<AccountCategory> _categories { get; set; }
    public IReadOnlyCollection<AccountSubCategory> SubCategories => [.. _subCategories];
    private ICollection<AccountSubCategory> _subCategories { get; set; }
    public IReadOnlyCollection<AccountPaymentMethod> PaymentMethods => [.. _paymentMethods];
    private ICollection<AccountPaymentMethod> _paymentMethods { get; set; }
    public IReadOnlyCollection<AccountUser> Users => [.. _users];
    private ICollection<AccountUser> _users { get; set; }
    public IReadOnlyCollection<Invite> Invites => [.. _invites];
    private ICollection<Invite> _invites { get; set; }

    private Account()
    {
        Id = AccountId.New();
        Name = string.Empty;
        Description = string.Empty;
        _categories = [];
        _subCategories = [];
        _paymentMethods = [];
        _users = [];
        _invites = [];
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

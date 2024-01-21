namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Entities.Accounts.DomainEvents;
using Family.Budget.Domain.Entities.Accounts.Exceptions;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.SeedWork;
using System.Collections.Immutable;

public class Account : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Currency Currency { get; private set; }
    public bool Deleted { get; private set; }
    public DateTimeOffset FirstMovement { get; private set; }
    public DateTimeOffset LastMovement { get; private set; }

    public IReadOnlyCollection<AccountCategory> Categories => _categories.ToImmutableArray();
    private readonly ICollection<AccountCategory> _categories;

    public IReadOnlyCollection<AccountSubCategory> SubCategories => _subCategories.ToImmutableArray();
    private readonly ICollection<AccountSubCategory> _subCategories;

    public IReadOnlyCollection<AccountPaymentMethod> PaymentMethods => _paymentMethods.ToImmutableArray();
    private readonly ICollection<AccountPaymentMethod> _paymentMethods;

    public IReadOnlyCollection<AccountUser> UserIds => _userIds.ToImmutableArray();
    private readonly ICollection<AccountUser> _userIds;

    public IReadOnlyCollection<Invite> Invites => _invites.ToImmutableArray();
    private readonly ICollection<Invite> _invites;

    private Account()
    {
        _categories = new HashSet<AccountCategory>();
        _subCategories = new HashSet<AccountSubCategory>();
        _paymentMethods = new HashSet<AccountPaymentMethod>();

        _userIds = new HashSet<AccountUser>();
        _invites = new HashSet<Invite>();
    }

    private Account(AccountId id,
        string name,
        string description,
        Currency currency,
        bool deleted,
        DateTimeOffset firstMovement,
        DateTimeOffset lastMovement)
    {
        Id = id;
        Name = name;
        Description = description;
        Currency = currency;
        Deleted = deleted;
        FirstMovement = firstMovement;
        LastMovement = lastMovement;

        _categories = new HashSet<AccountCategory>();
        _subCategories = new HashSet<AccountSubCategory>();
        _paymentMethods = new HashSet<AccountPaymentMethod>();

        _userIds = new HashSet<AccountUser>();
        _invites = new HashSet<Invite>();
    }

    public static Account New(string name,
        string description,
        ICollection<Category> categories,
        ICollection<SubCategory> subCategories,
        ICollection<PaymentMethod> paymentMethods,
        ICollection<Guid> users,
        Currency currency)
    {
        var entity = new Account(AccountId.New(), name, description, currency, false, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        categories.ToList().ForEach(x => entity._categories.Add(new AccountCategory(entity, x)));
        subCategories.ToList().ForEach(x => entity._subCategories.Add(new AccountSubCategory(entity, x)));
        paymentMethods.ToList().ForEach(x => entity._paymentMethods.Add(new AccountPaymentMethod(entity, x)));
        users.ToList().ForEach(x => entity._userIds.Add(new AccountUser(entity, x)));

        entity.RaiseDomainEvent(new AccountCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        base.Validate();
    }

    public void Delete()
    {
        Deleted = true;
        Validate();

        RaiseDomainEvent(new AccountDeletedDomainEvent(this));
    }

    public void SetFirstMovement(DateTimeOffset firstMovement)
    {
        FirstMovement = firstMovement;
        Validate();
    }

    public void SetLastMovement(DateTimeOffset lastMovement)
    {
        LastMovement = lastMovement;
        Validate();
    }

    public void InviteAFriend(string email)
    {
        if (Invites.Any(x => x.Email == email))
        {
            throw new UserAlreadInvitedToAccount("UserAlreadInvitedToAccount");
        }

        _invites.Add(Invite.New(email, InviteStatus.Draft, this));

        Validate();
    }

    public void ChangeEmailFriend(string email)
    {
        Invites.First(x => x.Email == email).SetStatus(InviteStatus.Sended);
    }

}

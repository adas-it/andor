namespace Family.Budget.Domain.Entities.Users;

using Family.Budget.Domain.Entities.Users.DomainEvents;
using Family.Budget.Domain.Entities.Users.ValueObject;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;

public class User : AggregateRoot
{
    public string UserName { get; private set; }
    public bool Enabled { get; private set; }
    public bool EmailVerified { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Avatar { get; private set; }
    public LocationInfos LocationInfos { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool AcceptedTermsCondition { get; private set; }
    public DateTimeOffset AcceptedTermsConditionDate { get; private set; }
    public bool AcceptedPrivateData { get; private set; }
    public DateTimeOffset AcceptedPrivateDataDate { get; private set; }

    private User()
    { }

    private User(Guid id, string userName, bool enabled, bool emailVerified, string firstName,
        string lastName, string email, string avatar, DateTimeOffset createdAtUtc,
        bool acceptedTermsCondition, DateTimeOffset acceptedTermsConditionDate, bool acceptedPrivateData,
        DateTimeOffset acceptedPrivateDataDate, LocationInfos locationInfos)
    {
        Id = id;
        UserName = userName;
        Enabled = enabled;
        EmailVerified = emailVerified;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Avatar = avatar;
        CreatedAt = createdAtUtc;
        AcceptedTermsCondition = acceptedTermsCondition;
        AcceptedTermsConditionDate = acceptedTermsConditionDate;
        AcceptedPrivateData = acceptedPrivateData;
        AcceptedPrivateDataDate = acceptedPrivateDataDate;
        LocationInfos = locationInfos;
    }

    public static User New(Guid? id, string userName, bool enabled, bool emailVerified,
        string firstName, string lastName, string email, string avatar,
        DateTimeOffset createdAtUtc, bool acceptedTermsCondition, DateTimeOffset acceptedTermsConditionDate,
        bool acceptedPrivateData, DateTimeOffset acceptedPrivateDataDate, LocationInfos locationInfos)
    {
        var entity = new User(id ?? Guid.NewGuid(),
            userName,
            enabled,
            emailVerified,
            firstName,
            lastName,
            email,
            avatar,
            createdAtUtc,
            acceptedTermsCondition,
            acceptedTermsConditionDate,
            acceptedPrivateData,
            acceptedPrivateDataDate, locationInfos);

        entity.RaiseDomainEvent(new UserCreatedDomainEvent(entity));

        return entity; 
    }

    protected override void Validate()
    {
        AddNotification(UserName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(UserName.BetweenLength(3, 50));

        base.Validate();
    }
}

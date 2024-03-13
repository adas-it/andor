using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Entities.Currencies;
using Andor.Domain.Entities.Languages;
using Andor.Domain.Entities.Users.DomainEvents;
using Andor.Domain.Entities.Users.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Entities.Users;

public class User : AggregateRoot<UserId>
{
    public string UserName { get; private set; } = string.Empty;
    public bool Enabled { get; private set; }
    public bool EmailVerified { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public MailAddress Email { get; private set; }
    public string Avatar { get; private set; } = string.Empty;
    public Currency? CurrencyPreferred { get; private set; }
    public Language? LanguagePreferred { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool AcceptedTermsCondition { get; private set; }
    public DateTime AcceptedTermsConditionDate { get; private set; }
    public bool AcceptedPrivateData { get; private set; }
    public DateTime AcceptedPrivateDataDate { get; private set; }

    public static User New(UserId? id, string userName, bool enabled, bool emailVerified,
        string firstName, string lastName, MailAddress email, string avatar,
        DateTime createdAt, bool acceptedTermsCondition, DateTime acceptedTermsConditionDate,
        bool acceptedPrivateData, DateTime acceptedPrivateDataDate, Currency currencyPreferred, Language languagePreferred)
    {
        var entity = new User()
        {
            Id = id ?? UserId.New(),
            UserName = userName,
            Enabled = enabled,
            EmailVerified = emailVerified,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Avatar = avatar,
            CreatedAt = createdAt,
            AcceptedTermsCondition = acceptedTermsCondition,
            AcceptedTermsConditionDate = acceptedTermsConditionDate,
            AcceptedPrivateData = acceptedPrivateData,
            AcceptedPrivateDataDate = acceptedPrivateDataDate,
            CurrencyPreferred = currencyPreferred,
            LanguagePreferred = languagePreferred,
        };

        entity.RaiseDomainEvent(new UserCreatedDomainEvent(entity));

        return entity;
    }

    protected override DomainResult Validate()
    {
        AddNotification(UserName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(UserName.BetweenLength(3, 50));

        return base.Validate();
    }
}

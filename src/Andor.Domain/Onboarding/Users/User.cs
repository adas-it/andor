using Andor.Domain.Administrations.Languages;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Onboarding.Users.DomainEvents;
using Andor.Domain.Onboarding.Users.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Onboarding.Users;

public class User : AggregateRoot<UserId>
{
    public string UserName { get; private set; } = string.Empty;
    public bool Enabled { get; private set; }
    public bool EmailVerified { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MailAddress Email { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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

        entity.RaiseDomainEvent(UserCreatedDomainEvent.FromAggregateRoot(entity));

        return entity;
    }

    protected override DomainResult Validate()
    {
        AddNotification(UserName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(UserName.BetweenLength(3, 50));

        return base.Validate();
    }
}

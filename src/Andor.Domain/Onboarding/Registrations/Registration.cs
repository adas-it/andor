using Andor.Domain.Administrations.Languages;
using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.Onboarding.Registrations.DomainEvents;
using Andor.Domain.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Onboarding.Registrations;

public class Registration : AggregateRoot<RegistrationId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string UserName { get; private set; }
    public MailAddress Email { get; private set; }
    public CheckCode CheckCode { get; private set; }
    public DateTime RegisterDate { get; private set; }
    public RegistrationState State { get; private set; }
    public LanguageId LanguageId { get; private set; }
    public Language Language { get; private set; }
    public CurrencyId CurrencyId { get; private set; }
    public Currency Currency { get; private set; }
    public Guid CountryId { get; private set; }

    private Registration()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        CheckCode = CheckCode.New();
        State = RegistrationState.Undefined;
    }

    private DomainResult SetValues(
        RegistrationId id,
        string firstName,
        string lastName,
        MailAddress email,
        string checkCode,
        DateTime registerDate,
        RegistrationState state,
        Language language,
        Currency currency,
        Guid countryId)
    {
        AddNotification(firstName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(firstName.BetweenLength(2, 50));

        AddNotification(lastName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(lastName.BetweenLength(2, 50));

        AddNotification(registerDate.NotDefaultDateTime());

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        FirstName = firstName;
        UserName = email.ToString();
        LastName = lastName;
        Email = email;
        CheckCode = checkCode;
        RegisterDate = registerDate;
        State = state;
        LanguageId = language.Id;
        Language = language;
        CurrencyId = currency.Id;
        Currency = currency;
        CountryId = countryId;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Registration?) New(
        string firstName,
        string lastName,
        MailAddress email,
        Language language,
        Currency currency,
        Guid countryId)
    {
        var entity = new Registration();

        var result = entity.SetValues(RegistrationId.New(),
        firstName,
        lastName,
        email,
        CheckCode.New(),
        DateTime.UtcNow,
        RegistrationState.GeneratedCode,
        language,
        currency,
        countryId);

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(RegistrationCreatedDomainEvent.FromAggregator(entity));

        return (result, entity);
    }

    public DomainResult Complete(string userName, string firstName,
        string lastName,
        bool acceptedTermsCondition,
        bool acceptedPrivateData,
        string password,
        Language language,
        Currency currency,
        Guid countryId
        )
    {
        var result = SetValues(Id,
            firstName,
            lastName,
            Email,
            CheckCode,
            RegisterDate,
            RegistrationState.Completed,
            language,
            currency,
            countryId);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(RegistrationCompletedDomainEvent.FromAggregator(this,
            userName, countryId, acceptedTermsCondition, acceptedPrivateData, password));

        return result;
    }

    public DomainResult SetNewCode()
    {
        var code = CheckCode.New();

        var result = SetValues(Id,
            FirstName,
            LastName,
            Email,
            code,
            RegisterDate,
            State,
            Language,
            Currency,
            CountryId);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(RegistrationCodeChangedDomainEvent.FromAggregator(this));

        return result;
    }

    public bool IsComplete()
        => State == RegistrationState.Completed;

    public bool IsTheRightCode(CheckCode code)
        => CheckCode.Equals(code);
}

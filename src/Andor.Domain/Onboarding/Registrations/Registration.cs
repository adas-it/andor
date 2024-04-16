using Andor.Domain.Common.ValuesObjects;
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
    public MailAddress Email { get; private set; }
    public CheckCode CheckCode { get; private set; }
    public DateTime RegisterDate { get; private set; }
    public RegistrationState State { get; private set; }

    private Registration()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = new MailAddress("");
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
        RegistrationState state)
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
        LastName = lastName;
        Email = email;
        CheckCode = checkCode;
        RegisterDate = registerDate;
        State = state;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Registration?) New(
        string firstName,
        string lastName,
        MailAddress email)
    {
        var entity = new Registration();

        var result = entity.SetValues(RegistrationId.New(),
        firstName,
        lastName,
        email,
        CheckCode.New(),
        DateTime.UtcNow,
        RegistrationState.GeneratedCode);

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(RegistrationCreatedDomainEvent.FromAggregator(entity));

        return (result, entity);
    }

    public DomainResult Complete(string userName, string firstName,
        string locale,
        string lastName,
        bool acceptedTermsCondition,
        bool acceptedPrivateData,
        string password)
    {
        var result = SetValues(Id, firstName, lastName, Email, CheckCode, RegisterDate, RegistrationState.Completed);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(RegistrationCompletedDomainEvent.FromAggregator(this,
            userName, locale, acceptedTermsCondition, acceptedPrivateData, password));

        return result;
    }

    public DomainResult SetNewCode()
    {
        var code = CheckCode.New();

        var result = SetValues(Id, FirstName, LastName, Email, code, RegisterDate, State);

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

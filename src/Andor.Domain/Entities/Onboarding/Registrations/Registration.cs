using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Entities.Onboarding.Registrations.DomainEvents;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Entities.Onboarding.Registrations;

public class Registration : AggregateRoot<RegistrationId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public MailAddress Email { get; private set; }
    public CheckCode Code { get; private set; }
    public DateTime RegisterDate { get; private set; }
    public RegistrationState State { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Registration()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
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
        AddNotification(FirstName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(FirstName.BetweenLength(2, 50));

        AddNotification(LastName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(LastName.BetweenLength(2, 50));

        AddNotification(RegisterDate.NotDefaultDateTime());

        var result = Validate();

        if (result.IsFailure)
        {
            return result;
        }

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Code = checkCode;
        RegisterDate = registerDate;
        State = state;

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

        entity.RaiseDomainEvent(new RegistrationCreatedDomainEvent(entity));

        return (result, entity);
    }

    public DomainResult SetNewCode()
    {
        Code = CheckCode.New();

        RaiseDomainEvent(new RegistrationCodeChangedDomainEvent(this));

        return Validate();
    }

    public bool IsComplete()
        => State == RegistrationState.Completed;

    public bool IsTheRightCode(CheckCode code)
        => Code.Equals(code);
}



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
    public CheckCode CheckCode { get; private set; }
    public DateTime RegisterDate { get; private set; }
    public RegistrationState State { get; private set; }
    private Registration()
    {
    }

    private Registration(
        RegistrationId id,
        string firstName,
        string lastName,
        MailAddress email,
        string checkCode,
        DateTime registerDate,
        RegistrationState state)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CheckCode = checkCode;
        RegisterDate = registerDate;
        State = state;

        Validate();
    }

    public static Registration New(
        string firstName,
        string lastName,
        MailAddress email)
    {
        var entity = new Registration(Guid.NewGuid(),
        firstName,
        lastName,
        email,
        CheckCode.New(),
        DateTime.UtcNow,
        RegistrationState.GeneratedCode);

        entity.RaiseDomainEvent(new RegistrationCreatedDomainEvent(entity));

        return entity;
    }

    protected override DomainResult Validate()
    {
        AddNotification(FirstName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(FirstName.BetweenLength(2, 50));

        AddNotification(LastName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(LastName.BetweenLength(2, 50));

        AddNotification(RegisterDate.NotDefaultDateTime());

        return base.Validate();
    }

    public DomainResult SetNewCode()
    {
        CheckCode = CheckCode.New();

        RaiseDomainEvent(new RegistrationCodeChangedDomainEvent(this));

        return Validate();
    }

    public bool IsComplete()
        => State == RegistrationState.Completed;

    public bool IsTheRightCheckCode(string code)
        => CheckCode.Equals(code);
}



namespace Family.Budget.Domain.Entities.Registrations;

using Family.Budget.Domain.Entities.Registrations.DomainEvents;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;
using System.Linq;

public class Registration : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string CheckCode { get; private set; }
    public DateTimeOffset RegisterDate { get; private set; }

    private Registration(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string checkCode,
        DateTimeOffset registerDate)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CheckCode = checkCode;
        RegisterDate = registerDate;

        Validate();
    }

    public static Registration New(
        string firstName,
        string lastName,
        string email)
    {
        var entity = new Registration(Guid.NewGuid(),
        firstName,
        lastName,
        email,
        GetNewCheckCode(),
        DateTime.UtcNow);

        entity.RaiseDomainEvent(new RegistrationCreatedDomainEvent(entity));

        return entity;

    }

    private static string GetNewCheckCode()
    {
        var random = new Random();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    protected override void Validate()
    {
        AddNotification(FirstName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(FirstName.BetweenLength(3, 100));

        AddNotification(LastName.NotNullOrEmptyOrWhiteSpace());
        AddNotification(LastName.BetweenLength(3, 1000));

        AddNotification(CheckCode.BetweenLength(4, 4));

        AddNotification(RegisterDate.NotDefaultDateTime());

        base.Validate();
    }

    public void SetNewCode()
    {
        CheckCode = GetNewCheckCode();

        RaiseDomainEvent(new RegistrationCodeChangedDomainEvent(this));

        Validate();
    }

    public bool IsTheRightCheckCode(string code)
        => CheckCode.ToLower().Equals(code.ToLower());
}


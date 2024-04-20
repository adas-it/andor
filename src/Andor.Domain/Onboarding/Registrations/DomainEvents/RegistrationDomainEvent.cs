namespace Andor.Domain.Onboarding.Registrations.DomainEvents;

public sealed record RegistrationCreatedDomainEvent
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string CheckCode { get; init; } = "";
    public DateTime RegisterDate { get; init; }
    public int State { get; init; }

    public static RegistrationCreatedDomainEvent FromAggregator(Registration entity)
        => new RegistrationCreatedDomainEvent() with
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Address,
            CheckCode = entity.CheckCode,
            RegisterDate = entity.RegisterDate,
            State = entity.State.Key
        };
}

public sealed record RegistrationCodeChangedDomainEvent
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string CheckCode { get; init; } = "";
    public DateTime RegisterDate { get; init; }
    public int State { get; init; }

    public static RegistrationCodeChangedDomainEvent FromAggregator(Registration entity)
        => new RegistrationCodeChangedDomainEvent() with
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Address,
            CheckCode = entity.CheckCode,
            RegisterDate = entity.RegisterDate,
            State = entity.State.Key
        };
}

public sealed record RegistrationCompletedDomainEvent
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Password { get; init; } = "";
    public string Email { get; init; } = "";
    public Guid Locale { get; init; }
    public Guid LanguageId { get; init; }
    public Guid CurrencyId { get; init; }
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }

    public static RegistrationCompletedDomainEvent FromAggregator(Registration entity,
        string userName, Guid locale, bool acceptedTermsCondition, bool acceptedPrivateData,
        string password)
        => new RegistrationCompletedDomainEvent() with
        {
            Id = entity.Id,
            UserName = userName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Address,
            Locale = locale,
            AcceptedTermsCondition = acceptedTermsCondition,
            AcceptedPrivateData = acceptedPrivateData,
            Password = password,
            LanguageId = entity.Language.Id,
            CurrencyId = entity.Currency.Id
        };
}

namespace Andor.Domain.Onboarding.Users.DomainEvents;

public record UserCreatedDomainEvent
{
    public Guid Id { get; init; }

    public string UserName { get; set; }
    public bool Enabled { get; set; }
    public bool EmailVerified { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public Guid PreferredCurrencyId { get; set; }
    public Guid PreferredLanguageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool AcceptedTermsCondition { get; set; }
    public DateTime AcceptedTermsConditionDate { get; set; }
    public bool AcceptedPrivateData { get; set; }
    public DateTime AcceptedPrivateDataDate { get; set; }

    public static UserCreatedDomainEvent FromAggregateRoot(User entity)
        => new UserCreatedDomainEvent() with
        {
            Id = entity.Id,
            UserName = entity.UserName,
            Enabled = entity.Enabled,
            EmailVerified = entity.EmailVerified,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Address,
            Avatar = entity.Avatar,
            PreferredCurrencyId = entity.CurrencyPreferred.Id,
            PreferredLanguageId = entity.LanguagePreferred.Id,
            CreatedAt = entity.CreatedAt,
            AcceptedTermsCondition = entity.AcceptedTermsCondition,
            AcceptedTermsConditionDate = entity.AcceptedTermsConditionDate,
            AcceptedPrivateData = entity.AcceptedPrivateData,
            AcceptedPrivateDataDate = entity.AcceptedPrivateDataDate,
        };
}

public record UserUpdatedDomainEvent
{
    public Guid Id { get; init; }
    public static UserUpdatedDomainEvent FromAggregateRoot(User entity)
        => new UserUpdatedDomainEvent() with
        {
            Id = entity.Id
        };
}

public record UserDeletedDomainEvent
{
    public Guid Id { get; init; }
    public static UserDeletedDomainEvent FromAggregateRoot(User entity)
        => new UserDeletedDomainEvent() with
        {
            Id = entity.Id
        };
}

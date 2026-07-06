using System.Net.Mail;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Users.Users.DomainEvents;
using Users.Users.ValueObjects;

namespace Users.Users;

public sealed class User : AggregateRoot<UserId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public MailAddress Email { get; private set; }
    public Guid PreferredCurrencyId { get; private set; }
    public Guid PreferredLanguageId { get; private set; }

    protected User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = default!;
    }

    private User(
        UserId id,
        MailAddress email,
        string firstName,
        string lastName,
        Guid preferredCurrencyId,
        Guid preferredLanguageId)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PreferredCurrencyId = preferredCurrencyId;
        PreferredLanguageId = preferredLanguageId;
    }

    public static async Task<(DomainResult, User?)> NewAsync(
        UserId userId,
        MailAddress email,
        string firstName,
        string lastName,
        Guid preferredCurrencyId,
        Guid preferredLanguageId,
        IUserValidator validator,
        CancellationToken cancellationToken)
    {
        var entity = new User(
            userId,
            email,
            firstName,
            lastName,
            preferredCurrencyId,
            preferredLanguageId);

        var result = entity.Validate();

        if (result.IsFailure)
        {
            return (result, null);
        }

        result = await entity.ValidateAsync(validator, cancellationToken);

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(UserCreatedDomainEvent.FromAggregateRoot(entity));

        return (result, entity);
    }
}

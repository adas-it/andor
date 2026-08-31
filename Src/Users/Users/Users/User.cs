using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Domain.Users;

public class User : AggregateRoot<UserId>
{
    public Name FirstName { get; private set; }
    public Name LastName { get; private set; }
    public Email Email { get; private set; }
    public Guid PreferredCurrencyId { get; private set; }
    public Guid PreferredLanguageId { get; private set; }

    protected User()
    {
        FirstName = Name.Empty;
        LastName = Name.Empty;
        Email = Email.Empty;
        PreferredCurrencyId = Guid.Empty;
        PreferredLanguageId = Guid.Empty;
    }

    private User(
        UserId id,
        Email email,
        Name firstName,
        Name lastName,
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
        Email email,
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

        //entity.RaiseDomainEvent(UserCreatedDomainEvent.FromAggregateRoot(entity));

        return (result, entity);
    }
}

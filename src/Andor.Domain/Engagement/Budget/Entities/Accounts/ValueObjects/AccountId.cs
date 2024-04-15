using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;

public record struct AccountId
{
    private AccountId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static AccountId New() => new(Guid.NewGuid());

    public static AccountId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new AccountId(guid);
    }

    public static AccountId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator AccountId(Guid value) => new(value);

    public static implicit operator Guid(AccountId id) => id.Value;
}

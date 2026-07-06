using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts.ValueObjects;

public readonly record struct AccountId : IId<AccountId>
{
    public static AccountId Empty => new AccountId() { Value = Guid.Empty };

    private AccountId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; init; }

    public static AccountId New() => new(Guid.NewGuid());

    public static AccountId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new AccountId(guid);
    }

    public static AccountId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator AccountId(Guid value) => new(value);

    public static implicit operator Guid(AccountId id) => id.Value;
}

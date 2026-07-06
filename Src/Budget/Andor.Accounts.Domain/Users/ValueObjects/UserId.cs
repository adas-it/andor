using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Users.ValueObjects;

public readonly record struct UserId : IId<UserId>
{
    public static UserId Empty => new UserId() { Value = Guid.Empty };

    private UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; init; }
    public static UserId New() => new(Guid.NewGuid());

    public static UserId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new UserId(guid);
    }

    public static UserId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator UserId(Guid value) => new(value);

    public static implicit operator Guid(UserId id) => id.Value;
}

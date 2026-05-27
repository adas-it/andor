using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Users.ValueObjects;

public record struct UserId : IId<UserId>
{
    private UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static UserId New() => new(Guid.NewGuid());

    public static UserId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new UserId(guid);
    }

    public static UserId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator UserId(Guid value) => new(value);

    public static implicit operator Guid(UserId id) => id.Value;
}

using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Users.Users.ValueObjects;

public readonly record struct UserId(Guid Value) : IId<UserId>
{
    public static UserId Empty => new UserId() { Value = Guid.Empty };

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

using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Users.ValueObjects;

public record struct UserId(Guid Value)
{
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


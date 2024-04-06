using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Communications.Users.ValueObjects;

public record struct RecipientId(Guid Value)
{
    public static RecipientId New() => new(Guid.NewGuid());

    public static RecipientId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new RecipientId(guid);
    }

    public static RecipientId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator RecipientId(Guid value) => new(value);

    public static implicit operator Guid(RecipientId id) => id.Value;
}

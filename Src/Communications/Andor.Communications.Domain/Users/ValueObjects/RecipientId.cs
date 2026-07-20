using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.Users.ValueObjects;

public readonly record struct RecipientId : IId<RecipientId>
{
    public static RecipientId Empty => new RecipientId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private RecipientId(Guid value)
    {
        Value = value;
    }

    public static RecipientId New() => new RecipientId(Guid.NewGuid());

    public static RecipientId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new RecipientId(guid);
    }

    public static RecipientId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator RecipientId(Guid value) => new(value);

    public static implicit operator Guid(RecipientId id) => id.Value;
}

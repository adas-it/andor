using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.ValueObjects;

public readonly record struct MessageId : IId<MessageId>
{
    public static MessageId Empty => new MessageId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private MessageId(Guid value)
    {
        Value = value;
    }

    public static MessageId New() => new MessageId(Guid.NewGuid());

    public static MessageId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new MessageId(guid);
    }

    public static MessageId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator MessageId(Guid value) => new(value);

    public static implicit operator Guid(MessageId id) => id.Value;
}

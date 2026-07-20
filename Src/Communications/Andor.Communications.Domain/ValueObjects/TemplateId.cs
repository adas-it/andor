using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.ValueObjects;

public readonly record struct TemplateId : IId<TemplateId>
{
    public static TemplateId Empty => new TemplateId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private TemplateId(Guid value)
    {
        Value = value;
    }

    public static TemplateId New() => new TemplateId(Guid.NewGuid());

    public static TemplateId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new TemplateId(guid);
    }

    public static TemplateId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator TemplateId(Guid value) => new(value);

    public static implicit operator Guid(TemplateId id) => id.Value;
}


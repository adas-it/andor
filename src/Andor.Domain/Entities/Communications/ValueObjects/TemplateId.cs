using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Communications.ValueObjects;

public record struct TemplateId(Guid Value)
{
    public static TemplateId New() => new(Guid.NewGuid());

    public static TemplateId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new TemplateId(guid);
    }

    public static TemplateId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator TemplateId(Guid value) => new(value);

    public static implicit operator Guid(TemplateId id) => id.Value;
}


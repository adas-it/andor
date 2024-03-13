using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Languages.ValueObjects;

public record struct LanguageId(Guid Value)
{
    public static LanguageId New() => new(Guid.NewGuid());

    public static LanguageId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new LanguageId(guid);
    }

    public static LanguageId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator LanguageId(Guid value) => new(value);

    public static implicit operator Guid(LanguageId id) => id.Value;
}

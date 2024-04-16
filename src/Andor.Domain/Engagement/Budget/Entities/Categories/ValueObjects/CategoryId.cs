using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.Categories.ValueObjects;

public record struct CategoryId
{
    private CategoryId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static CategoryId New() => new(Guid.NewGuid());

    public static CategoryId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new CategoryId(guid);
    }

    public static CategoryId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator CategoryId(Guid value) => new(value);

    public static implicit operator Guid(CategoryId id) => id.Value;
}

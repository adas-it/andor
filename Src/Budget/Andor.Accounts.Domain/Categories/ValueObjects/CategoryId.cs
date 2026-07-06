using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Categories.ValueObjects;

public readonly record struct CategoryId : IId<CategoryId>
{
    public static CategoryId Empty => new CategoryId() { Value = Guid.Empty };

    private CategoryId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; init; }
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

    public readonly override string ToString() => Value.ToString();

    public static implicit operator CategoryId(Guid value) => new(value);

    public static implicit operator Guid(CategoryId id) => id.Value;
}

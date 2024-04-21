using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;

public record struct SubCategoryId
{
    private SubCategoryId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static SubCategoryId New() => new(Guid.NewGuid());

    public static SubCategoryId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new SubCategoryId(guid);
    }

    public static SubCategoryId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator SubCategoryId(Guid value) => new(value);

    public static implicit operator Guid(SubCategoryId id) => id.Value;
}
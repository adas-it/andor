using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;

public record struct CurrencyId
{
    private CurrencyId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static CurrencyId New() => new(Guid.NewGuid());

    public static CurrencyId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new CurrencyId(guid);
    }

    public static CurrencyId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator CurrencyId(Guid value) => new(value);

    public static implicit operator Guid(CurrencyId id) => id.Value;
}

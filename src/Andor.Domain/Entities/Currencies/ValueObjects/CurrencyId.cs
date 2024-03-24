using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Currencies.ValueObjects;

public record struct CurrencyId
{
    public Guid Value { get; private set; }
    public static CurrencyId New() => new CurrencyId() with { Value = Guid.NewGuid() };

    public static CurrencyId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new CurrencyId() with { Value = guid };
    }

    public static CurrencyId Load(Guid value) => new CurrencyId() with { Value = value };

    public override readonly string ToString() => Value.ToString();

    public static implicit operator CurrencyId(Guid value) => new CurrencyId() with { Value = value };

    public static implicit operator Guid(CurrencyId id) => id.Value;
}
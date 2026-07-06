using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Currencies.ValueObjects;

public readonly record struct CurrencyId : IId<CurrencyId>
{
    public static CurrencyId Empty => new CurrencyId() { Value = Guid.Empty };

    private CurrencyId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; init; }

    public static CurrencyId New() => new(Guid.NewGuid());

    public static CurrencyId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new CurrencyId(guid);
    }

    public static CurrencyId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator CurrencyId(Guid value) => new(value);

    public static implicit operator Guid(CurrencyId id) => id.Value;
}

using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.FinancialMovements.ValueObjects;

public record struct FinancialMovementId : IId<FinancialMovementId>
{
    private FinancialMovementId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static FinancialMovementId New() => new(Guid.NewGuid());

    public static FinancialMovementId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new FinancialMovementId(guid);
    }

    public static FinancialMovementId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator FinancialMovementId(Guid value) => new(value);

    public static implicit operator Guid(FinancialMovementId id) => id.Value;
}

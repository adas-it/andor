using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.FinancialMovements.ValueObjects;

public readonly record struct FinancialMovementId : IId<FinancialMovementId>
{
    public static FinancialMovementId Empty => new FinancialMovementId() { Value = Guid.Empty };

    private FinancialMovementId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; init; }

    public static FinancialMovementId New() => new(Guid.NewGuid());

    public static FinancialMovementId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new FinancialMovementId(guid);
    }

    public static FinancialMovementId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator FinancialMovementId(Guid value) => new(value);

    public static implicit operator Guid(FinancialMovementId id) => id.Value;
}

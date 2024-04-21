using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;

public record struct CashFlowId
{
    private CashFlowId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static CashFlowId New() => new(Guid.NewGuid());

    public static CashFlowId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new CashFlowId(guid);
    }

    public static CashFlowId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator CashFlowId(Guid value) => new(value);

    public static implicit operator Guid(CashFlowId id) => id.Value;
}

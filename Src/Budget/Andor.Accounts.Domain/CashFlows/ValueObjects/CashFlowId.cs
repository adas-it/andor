using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.CashFlows.ValueObjects;

public readonly record struct CashFlowId : IId<CashFlowId>
{
    public static CashFlowId Empty => new CashFlowId() { Value = Guid.Empty };

    private CashFlowId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; init; }

    public static CashFlowId New() => new(Guid.NewGuid());

    public static CashFlowId Load(string value)
    {
        return !Guid.TryParse(value, out var guid) ?
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value)) : new CashFlowId(guid);
    }

    public static CashFlowId Load(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator CashFlowId(Guid value) => new(value);

    public static implicit operator Guid(CashFlowId id) => id.Value;
}

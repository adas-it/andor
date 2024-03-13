namespace Family.Budget.Domain.Entities.Accounts.ValueObject;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Exceptions;
using System;

public record AccountId : IComparable<AccountId>
{
    public Guid Value { get; init; }

    public AccountId(Guid value)
    {
        Validate(value);
        Value = value;
    }

    private static void Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidDomainException(
                DefaultsErrorsMessages.NotNull.GetMessage(nameof(value)),
                CommonErrorCodes.Validation);
        }
    }

    public static AccountId New() => new (Guid.NewGuid());
    public override string ToString() => Value.ToString();

    public int CompareTo(AccountId? other) => Value.CompareTo(other?.Value);

    public static implicit operator AccountId(Guid value) => new(value);

    public static implicit operator Guid(AccountId accountId) => accountId.Value;
}

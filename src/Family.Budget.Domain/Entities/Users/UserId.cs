namespace Family.Budget.Domain.Entities.Users;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Exceptions;
using System;

public record UserId : IComparable<UserId>
{
    public Guid Value { get; init; }

    public UserId(Guid value)
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

    public static UserId New() => new (Guid.NewGuid());
    public override string ToString() => Value.ToString();

    public int CompareTo(UserId? other) => Value.CompareTo(other?.Value);

    public static implicit operator UserId(Guid value) => new(value);

    public static implicit operator Guid(UserId accountId) => accountId.Value;
}

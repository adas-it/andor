namespace Family.Budget.Domain.Entities.Accounts.Errors;

using Family.Budget.Domain.Common.ValuesObjects;

public sealed record AccountsErrorCodes : DomainErrorCode
{
    private AccountsErrorCodes(int Original) : base(Original) { }

    public static readonly AccountsErrorCodes Validation = new(2_000);
    public static readonly AccountsErrorCodes UserAlreadAddedToAccount = new(2_001);

    public static implicit operator AccountsErrorCodes(int value) => new(value);
    public static implicit operator int(AccountsErrorCodes month) => month.Value;
}

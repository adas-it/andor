namespace Family.Budget.Domain.Entities.Accounts.Exceptions;

using Family.Budget.Domain.Entities.Accounts.Errors;
using System;

public class UserAlreadInvitedToAccount : Exception
{
    public int Code { get; init; }

    public UserAlreadInvitedToAccount(string? message) : base(message)
    {
        Code = AccountsErrorCodes.UserAlreadAddedToAccount;
    }
}

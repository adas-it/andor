using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts;

public interface IAccountValidator : IDefaultValidator<Account, AccountId>
{
    Task<List<Notification>> ValidateUpdateAsync(Account account,
        CancellationToken cancellationToken);
}

using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts;

public interface IAccountValidator : IDefaultValidator<Account, AccountId>
{
    Task<List<Notification>> ValidateUpdateAsync(Account account, Name name, Description description,
        Currency currency, CancellationToken cancellationToken);
}

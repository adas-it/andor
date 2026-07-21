using Andor.Accounts.Contracts;
using Andor.Accounts.Domain.Accounts;

namespace Andor.Accounts.Application;

internal static class AccountMapperExtensions
{
    public static AccountOutput? ToAccountOutput(this Account? entity)
    {
        if (entity == null)
            return null;

        return new AccountOutput(entity.Id.ToString(), entity.Name, entity.Currency.Id.ToString());
    }
}

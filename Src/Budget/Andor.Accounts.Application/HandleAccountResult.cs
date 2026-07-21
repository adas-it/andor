using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application;

public static class HandleAccountResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
    {
        { AccountErrorCode.AccountShouldHaveOneOwner, AccountErrors.AccountValidation() },
        { AccountErrorCode.CurrencyNotFound, AccountErrors.CurrencyNotFound() },
    };

    public static async Task HandleResultAccount<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class?
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

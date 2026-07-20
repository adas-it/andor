using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain.Errors;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Application;

public static class HandleRuleResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
        {
            { CommunicationsErrorCodes.RuleValidation, RuleErrors.RuleValidation() },
            { CommunicationsErrorCodes.RuleNotFound, RuleErrors.RuleNotFound() },
            { CommunicationsErrorCodes.ActionNotAllowed, RuleErrors.ActionNotAllowed() },
        };

    public static async Task HandleResultRule<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class?
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

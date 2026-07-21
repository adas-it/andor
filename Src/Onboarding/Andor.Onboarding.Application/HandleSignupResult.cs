using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.Errors;
using Andor.Onboarding.Contracts.Responses;

namespace Andor.Onboarding.Application;

public static class HandleSignupResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
        {
            { SignupErrorCodes.SignupNotFound, SignupErrors.SignupNotFound() },
            { SignupErrorCodes.InvalidCode, SignupErrors.InvalidCode() },
            { SignupErrorCodes.CodeExpired, SignupErrors.CodeExpired() },
            { SignupErrorCodes.AlreadyVerified, SignupErrors.AlreadyVerified() },
        };

    public static async Task HandleResultSignup<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class?
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

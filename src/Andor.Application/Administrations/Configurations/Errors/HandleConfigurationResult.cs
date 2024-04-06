using Andor.Application.Common;
using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Common.ValuesObjects;

namespace Andor.Application.Administrations.Configurations.Errors;

public static class HandleConfigurationResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
        {
            { DomainErrorCode.Validation, Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation()}
        };

    public static async Task HandleResultConfiguration<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

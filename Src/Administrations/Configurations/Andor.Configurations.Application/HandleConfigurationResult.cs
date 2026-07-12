using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain.Errors;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Application;

public static class HandleConfigurationResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
        {
            { ConfigurationsErrorCodes.ErrorOnChangeName, ConfigurationErrors.ConfigurationValidation() },
            { ConfigurationsErrorCodes.ThereWillCurrentConfigurationStartDate, ConfigurationErrors.ThereWillCurrentConfigurationStartDate() },
        };

    public static async Task HandleResultConfiguration<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class?
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

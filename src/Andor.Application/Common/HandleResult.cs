using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Domain.Common.ValuesObjects;

namespace Andor.Application.Common;

public static class HandleResult
{
    public static async Task Handle<T>(DomainResult result,
        ApplicationResult<T> notifier,
        Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class
    {
        var tasks = new List<Task>();

        tasks.Add(ErrorsHandler(result, notifier, errorsMapping));

        tasks.Add(WarningsHandler(result, notifier, errorsMapping));

        await Task.WhenAll(tasks);
    }

    private static Task ErrorsHandler<T>(DomainResult result, ApplicationResult<T> notifier, Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class
    {
        foreach (var error in result.Errors)
        {
            errorsMapping.TryGetValue(error.Error, out var value);

            if (value != null)
            {
                notifier.AddError(value
                    .ChangeInnerMessage(error.Message ?? string.Empty));
            }
            else
            {
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }

        return Task.CompletedTask;
    }

    private static Task WarningsHandler<T>(DomainResult result, ApplicationResult<T> notifier, Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class
    {
        foreach (var warning in result.Warnings)
        {
            errorsMapping.TryGetValue(warning.Error, out var value);

            if (value != null)
            {
                notifier.AddError(value);
            }
            else
            {
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }

        return Task.CompletedTask;
    }
}

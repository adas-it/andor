using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Application;

public static class HandleResult
{
    public static async Task Handle<T>(DomainResult result,
        ApplicationResult<T> notifier,
        Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class?
    {
        var tasks = new List<Task>
        {
            ErrorsHandler(result, notifier, errorsMapping),
            WarningsHandler(result, notifier, errorsMapping)
        };

        await Task.WhenAll(tasks);
    }

    private static Task ErrorsHandler<T>(DomainResult result, ApplicationResult<T> notifier,
        Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class?
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
                notifier.AddError(Errors.Generic());
            }
        }

        return Task.CompletedTask;
    }

    private static Task WarningsHandler<T>(DomainResult result, ApplicationResult<T> notifier,
        Dictionary<DomainErrorCode, ErrorModel> errorsMapping) where T : class?
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
                notifier.AddError(Errors.Generic());
            }
        }

        return Task.CompletedTask;
    }
}

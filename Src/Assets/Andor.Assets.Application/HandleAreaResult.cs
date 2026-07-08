using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Application;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Application;

public static class HandleAreaResult
{
    private static readonly Dictionary<DomainErrorCode, ErrorModel> _errorsMapping = new()
    {
    };

    public static async Task HandleResultConfiguration<T>(DomainResult result,
        ApplicationResult<T> notifier) where T : class?
    {
        await HandleResult.Handle<T>(result, notifier, _errorsMapping);
    }
}

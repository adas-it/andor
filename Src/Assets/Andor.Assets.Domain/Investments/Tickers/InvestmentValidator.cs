using Andor.Assets.Domain.Investments.Tickers.ValueObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Tickers;

public class TickerValidator()
    : DefaultValidator<Ticker, TickerId>, ITickerValidator
{
    public override async Task<List<Notification>> ValidateCreationAsync(Ticker entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        AddNotification(entity.Code.NotNull(), notifications);

        return notifications;

    }

    public Task<List<Notification>> ValidateUpdateAsync(Ticker account,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        return Task.FromResult(notifications);
    }
}


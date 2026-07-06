using Andor.Assets.Domain.Investments.Tickers.ValueObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Tickers;

public interface ITickerValidator : IDefaultValidator<Ticker, TickerId>
{
    Task<List<Notification>> ValidateUpdateAsync(Ticker ticker,
        CancellationToken cancellationToken);
}

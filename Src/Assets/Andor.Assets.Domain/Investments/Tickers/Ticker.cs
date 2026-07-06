using Andor.Assets.Domain.Investments.Movements;
using Andor.Assets.Domain.Investments.Tickers.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Tickers;

public class Ticker : AggregateRoot<TickerId>, ISoftDeletableEntity
{
    public string Code { get; private set; } = string.Empty;
    public decimal Quotas { get; private set; } = 0;
    public bool IsDeleted { get; private set; }

    public IReadOnlyCollection<Movement> Movements => [.. _movements];
    private ICollection<Movement> _movements
    {
        get; set;
    } = new HashSet<Movement>();

    protected Ticker()
    {
    }

    private Ticker(TickerId investmentId, string code, decimal quotas, bool isDeleted)
    {
        Id = investmentId;
        Code = code;
        Quotas = quotas;
        IsDeleted = isDeleted;
    }

    public static async Task<(DomainResult, Ticker?)> NewAsync(
        TickerId investmentId,
        string code,
        decimal quotas,
        ITickerValidator validator,
        CancellationToken cancellationToken)
    {
        var entity = new Ticker(investmentId, code, quotas, false);

        var result = await entity.ValidateAsync(validator, cancellationToken);

        if (result.IsSuccess)
        {
            //entity.RaiseDomainEvent(AccountCreatedDomainEvent.FromAggregator(entity, userId));
        }

        return (result, result.IsSuccess ? entity : null);
    }

}

using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Assets.Domain.Investments.Tickers;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;

namespace Andor.Assets.Domain.Investments.Areas;

public class Area : AggregateRoot<AreaId>, ISoftDeletableEntity
{
    public static Area Empty => new Area();

    public IReadOnlyCollection<Ticker> Tickers => [.. _tickers];
    private ICollection<Ticker> _tickers = new HashSet<Ticker>();


    public IReadOnlyCollection<Guid> Members => [.. _members];
    private ICollection<Guid> _members = new HashSet<Guid>();

    public bool IsDeleted { get; } = false;

    protected Area()
    {
    }
}

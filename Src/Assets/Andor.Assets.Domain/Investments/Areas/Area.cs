using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Assets.Domain.Investments.Tickers;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Areas;

public class Area : AggregateRoot<AreaId>, ISoftDeletableEntity
{
    public static Area Empty => new Area();

    public Name Name { get; set; } = Name.Empty;

    public IReadOnlyCollection<Ticker> Tickers => [.. _tickers];
    private ICollection<Ticker> _tickers = new HashSet<Ticker>();


    public IReadOnlyCollection<Guid> Members => [.. _members];
    private ICollection<Guid> _members = new HashSet<Guid>();

    public bool IsDeleted { get; } = false;

    protected Area()
    {
    }

    public static Task<(DomainResult, Area?)> NewAsync(
        AreaId id,
        Name name,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = new Area()
        {
            Id = id,
            Name = name,
            _members = new List<Guid>()
            {
                userId
            }
        };


        return Task.FromResult<(DomainResult, Area?)>((DomainResult.Success(), entity));
    }
}

using Andor.Assets.Domain.Investments.Movements.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Domain.Investments.Movements;

public class Movement : Entity<MovementId>
{
    public DateTime Date { get; set; } = DateTime.MinValue;

    public decimal Price { get; set; } = decimal.Zero;

    public decimal Quotas { get; set; } = decimal.Zero;

    public decimal Value { get; set; } = decimal.Zero;

    protected Movement()
    {
    }

    private Movement(MovementId id, DateTime date, decimal price, decimal quotas, decimal value)
    {
        Id = id;
        Date = date;
        Price = price;
        Quotas = quotas;
        Value = value;
    }

    public static (DomainResult, Movement?) New(
        MovementId id,
        DateTime date,
        decimal price,
        decimal quotas,
        decimal value)

    {
        var entity = new Movement(id, date, price, quotas, value);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

}

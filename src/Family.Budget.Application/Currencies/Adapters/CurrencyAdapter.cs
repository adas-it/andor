namespace Family.Budget.Application.Currencies.Adapters;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Domain.Entities.Currencies;

public static class CurrencyAdapter
{
    public static CurrencyOutput MapDtoFromDomain(this Currency item)
        => new(item.Id, item.Name, item.Iso);
}

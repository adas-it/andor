namespace Family.Budget.Application.Currencies.Services;

using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.Currencies.Repository;
using System.Linq;
using System.Threading.Tasks;

public class Currencieservices : ICurrencieservices
{
    private readonly ICurrencyRepository CurrencyRepository;
    private readonly Notifier notifier;

    public Currencieservices(ICurrencyRepository CurrencyRepository,
        Notifier notifier)
    {
        this.CurrencyRepository = CurrencyRepository;
        this.notifier = notifier;
    }

    public async Task Handle(Currency entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await CurrencyRepository.GetByName(entity.Name, cancellationToken);

        if (listWithSameName is not null && listWithSameName.Where(x => x.Id != entity.Id).Any())
        {
        }
    }
}

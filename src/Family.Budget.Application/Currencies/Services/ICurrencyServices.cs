namespace Family.Budget.Application.Currencies.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.Currencies;

public interface ICurrencieservices
{
    Task Handle(Currency entity, CancellationToken cancellationToken);
}

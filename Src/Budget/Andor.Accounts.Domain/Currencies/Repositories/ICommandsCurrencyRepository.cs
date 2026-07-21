using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.Currencies.Repositories;

public interface ICommandsCurrencyRepository : ICommandRepository<Currency, CurrencyId>
{
    Task<Currency?> GetByIsoAsync(string iso, CancellationToken cancellationToken);
}

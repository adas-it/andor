using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Currencies.Repositories;

public interface ICommandsCurrencyRepository : ICommandRepository<Currency, CurrencyId>
{
}

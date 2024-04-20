using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface ICommandsCurrencyRepository : ICommandRepository<Currency, CurrencyId>
{
}

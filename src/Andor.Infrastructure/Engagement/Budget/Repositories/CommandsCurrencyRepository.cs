using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsCurrencyRepository(PrincipalContext context) :
    CommandsBaseRepository<Currency, CurrencyId>(context),
    ICommandsCurrencyRepository
{
}

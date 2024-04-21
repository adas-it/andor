using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsCurrencyRepository(PrincipalContext context) :
    CommandsBaseRepository<Currency, CurrencyId>(context),
    ICommandsCurrencyRepository
{
}

using Andor.Domain.Engagement.Budget.Entities.Accounts;
using Andor.Domain.Engagement.Budget.Entities.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsAccountRepository(PrincipalContext context) :
    CommandsBaseRepository<Account, AccountId>(context),
    ICommandsAccountRepository
{
    protected readonly DbSet<Currency> _dbCurrencySet = context.Set<Currency>();

    public override Task InsertAsync(Account entity, CancellationToken cancellationToken)
    {
        if (entity.Currency is not null)
        {
            _dbCurrencySet.Attach(entity.Currency!);
        }

        return base.InsertAsync(entity, cancellationToken);
    }
}

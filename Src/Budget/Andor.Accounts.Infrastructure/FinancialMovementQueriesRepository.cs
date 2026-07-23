using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Infrastructure.Context;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Domain.ValuesObjects;
using Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Andor.Accounts.Infrastructure;

public class FinancialMovementQueriesRepository :
    QueryHelper<FinancialMovement, FinancialMovementId>, IFinancialMovementQueriesRepository
{
    public FinancialMovementQueriesRepository(AccountsContext context,
        ICurrentUserService _currentUserService) : base(context)
    {
        loggedUserFilter = x => x.Account.Members.Any(z => z.UserId == _currentUserService.GetCurrentUser().UserId);
    }

    public async Task<SearchOutput<FinancialMovement>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FinancialMovement>> GetAllFinancialMovementsByMonth(AccountId accountId, Month month, Year year, CancellationToken cancellationToken)
    {
        return await DbSet.Where(x => x.AccountId == accountId && x.Date.Month == month && x.Date.Year == year).ToListAsync(cancellationToken);
    }
}

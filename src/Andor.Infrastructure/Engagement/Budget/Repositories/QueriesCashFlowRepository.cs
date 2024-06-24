using Andor.Application.Common.Models.Authorizations;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesCashFlowRepository :
        QueryHelper<CashFlow, CashFlowId>, IQueriesCashFlowRepository
{
    public QueriesCashFlowRepository(PrincipalContext context,
        ICurrentUserService _currentUserService) : base(context)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == _currentUserService.User.UserId);
    }

    public Task<SearchOutput<CashFlow>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<CashFlow, bool>> where = x => true;

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<CashFlow>(input.Page, input.PerPage, total, items!));
    }

    public Task<CashFlow?> GetByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken)
    {
        return null;
    }

    public Task<CashFlow?> GetPreviousCashFlowByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken)
    {
        return null;
    }

    public Task<CashFlow?> GetNextCashFlowByAccountIdAsync(Year year, Month month, CancellationToken cancellationToken)
    {
        return null;
    }

    public async Task<CashFlow?> GetCurrentOrPreviousCashFlowByAccountIdAsync(AccountId accountId, Year year, Month month, CancellationToken cancellationToken)
        => await _dbSet.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
        .Where(loggedUserFilter)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.AccountId == accountId &&
            ((x.Year == year && x.Month <= month) || (x.Year < year)), cancellationToken);

    Task<SearchOutput<CashFlow>> IResearchableRepository<CashFlow, CashFlowId, SearchInput>.SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        return null;
    }
}

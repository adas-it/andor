using Andor.Application.Common.Models.Authorizations;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesAccountSubCategoryRepository : IQueriesAccountSubCategoryRepository
{
    protected readonly DbSet<AccountSubCategory> _dbSet;
    protected Expression<Func<AccountSubCategory, bool>>? loggedUserFilter;

    public QueriesAccountSubCategoryRepository(PrincipalContext context,
        ICurrentUserService _currentUserService)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == _currentUserService.User.UserId);
        _dbSet = context.Set<AccountSubCategory>();
    }

    public async Task<SubCategory?> GetByIdAsync(AccountId accountId, SubCategoryId subCategoryId,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();

        if (loggedUserFilter != null)
        {
            query = query.Where(loggedUserFilter);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubCategoryId == subCategoryId
                && x.AccountId == accountId, cancellationToken)
            .Select(x => x.SubCategory);
    }

    public Task<SearchOutput<SubCategory>> SearchAsync(SearchInputSubCategory input, CancellationToken cancellationToken)
    {
        Expression<Func<AccountSubCategory, bool>> where = x => x.AccountId == input.AccountId;

        var query = Extension.GetManyPaginated(
            _dbSet,
            loggedUserFilter,
            where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .Select(x => x.SubCategory);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            query = query.Where(x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (input.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == input.CategoryId.Value);
        }

        var items = query.ToList();

        return Task.FromResult(new SearchOutput<SubCategory>(input.Page, input.PerPage, total, items!));
    }
}

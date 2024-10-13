using Andor.Application.Common.Interfaces;
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

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

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
        List<Expression<Func<AccountSubCategory, bool>>> where = [];

        where.Add(x => x.AccountId == input.AccountId);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where.Add(x => x.SubCategory.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (input.CategoryId.HasValue)
        {
            where.Add(x => x.SubCategory.CategoryId == input.CategoryId.Value);
        }

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

        var items = query.ToList();

        return Task.FromResult(new SearchOutput<SubCategory>(input.Page, input.PerPage, total, items!));
    }
}

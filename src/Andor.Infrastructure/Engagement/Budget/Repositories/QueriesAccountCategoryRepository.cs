using Andor.Application.Common.Models.Authorizations;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesAccountCategoryRepository : IQueriesAccountCategoryRepository
{
    protected readonly DbSet<AccountCategory> _dbSet;
    protected Expression<Func<AccountCategory, bool>>? loggedUserFilter;

    public QueriesAccountCategoryRepository(PrincipalContext context,
        ICurrentUserService _currentUserService)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == _currentUserService.User.UserId);
        _dbSet = context.Set<AccountCategory>();
    }

    public async Task<Category?> GetByIdAsync(AccountId accountId, CategoryId categoryId, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();

        if (loggedUserFilter != null)
        {
            query = query.Where(loggedUserFilter);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CategoryId == categoryId
                && x.AccountId == accountId, cancellationToken)
            .Select(x => x.Category);
    }

    public Task<SearchOutput<Category>> SearchAsync(SearchInputCategory input, CancellationToken cancellationToken)
    {
        Expression<Func<AccountCategory, bool>> where = x => x.Category.Type == input.Type;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Category.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase)
                && x.Category.Type == input.Type;

        var items = Extension.GetManyPaginated(
            _dbSet,
            loggedUserFilter,
            where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .Select(x => x.Category)
            .ToList();

        return Task.FromResult(new SearchOutput<Category>(input.Page, input.PerPage, total, items!));
    }
}

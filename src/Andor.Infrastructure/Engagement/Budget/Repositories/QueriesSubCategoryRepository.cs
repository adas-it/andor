using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class QueriesSubCategoryRepository(PrincipalContext context) :
    QueryHelper<SubCategory, SubCategoryId>(context),
    IQueriesSubCategoryRepository
{
    public Task<SearchOutput<SubCategory>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<SubCategory, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<SubCategory>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<SubCategory>> GetByIdsAsync(List<SubCategoryId> ids, CancellationToken cancellationToken)
    {
        var items = _dbSet.Where(x => ids.Contains(x.Id))
            .Include(x => x.Category)
            .ToList();

        return Task.FromResult(items);
    }
}

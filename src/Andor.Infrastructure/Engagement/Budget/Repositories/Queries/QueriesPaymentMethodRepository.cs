using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

public class QueriesPaymentMethodRepository(PrincipalContext context) :
    QueryHelper<PaymentMethod, PaymentMethodId>(context),
    IQueriesPaymentMethodRepository
{
    public Task<SearchOutput<PaymentMethod>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<PaymentMethod, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<PaymentMethod>(input.Page, input.PerPage, total, items!));
    }

    public async Task<List<PaymentMethod>> GetManyByIdsAsync(List<PaymentMethodId> ids, CancellationToken cancellationToken)
        => await _dbSet.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
}

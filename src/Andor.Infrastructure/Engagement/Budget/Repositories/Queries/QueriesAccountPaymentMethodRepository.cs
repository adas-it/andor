using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

public class QueriesAccountPaymentMethodRepository : IQueriesAccountPaymentMethodRepository
{
    protected readonly DbSet<AccountPaymentMethod> _dbSet;
    protected Expression<Func<AccountPaymentMethod, bool>>? loggedUserFilter;

    public QueriesAccountPaymentMethodRepository(PrincipalContext context,
        ICurrentUserService _currentUserService)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == _currentUserService.User.UserId);
        _dbSet = context.Set<AccountPaymentMethod>();
    }

    public async Task<PaymentMethod?> GetByIdAsync(AccountId accountId, PaymentMethodId paymentMethodId, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();

        if (loggedUserFilter != null)
        {
            query = query.Where(loggedUserFilter);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethodId
                && x.AccountId == accountId, cancellationToken)
            .Select(x => x.PaymentMethod);
    }

    public Task<ListPaymentMethodsOutput> SearchAsync(SearchInputAccountPayment input, CancellationToken cancellationToken)
    {
        List<Expression<Func<AccountPaymentMethod, bool>>> where = [];

        where.Add(x => x.PaymentMethod.Type == input.Type);
        where.Add(x => x.AccountId == input.AccountId);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where.Add(x => x.PaymentMethod.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase));
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
            .Select(GetProjection())
            .OrderBy(x => x.Order);

        var items = query.ToList(); ;

        return Task.FromResult(new ListPaymentMethodsOutput(input.Page, input.PerPage, total, items!));
    }
    private static Expression<Func<AccountPaymentMethod, PaymentMethodOutput>> GetProjection()
    {
        return x => new PaymentMethodOutput()
        {
            Id = x.PaymentMethod.Id,
            Name = x.PaymentMethod.Name,
            Description = x.PaymentMethod.Description,
            StartDate = x.PaymentMethod.StartDate,
            DeactivationDate = x.PaymentMethod.DeactivationDate,
            Order = x.Order
        };
    }
}

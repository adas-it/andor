using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Application.Dto.Engagement.Budget.FinancialMovementStatuses;
using Andor.Application.Dto.Engagement.Budget.MovementTypes;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

public class QueriesFinancialMovementRepository :
    QueryHelper<FinancialMovement, FinancialMovementId>,
    IQueriesFinancialMovementRepository
{
    public QueriesFinancialMovementRepository(PrincipalContext context,
        ICurrentUserService currentUserService) : base(context)
    {
        loggedUserFilter = x => x.Account.Users.Any(z => z.UserId == currentUserService.User.UserId);
    }

    public async Task<List<FinancialMovementOutput>> GetAllFinancialMovementsByMonth(
        Domain.Engagement.Budget.Accounts.Accounts.ValueObjects.AccountId accountId,
        Year year,
        Month month,
        CancellationToken cancellationToken)
    {
        DateTime startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(-1).ToUniversalTime();
        DateTime endDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddMilliseconds(-1).ToUniversalTime();

        var query = _dbSet.AsNoTracking();

        query = query.Where(x => x.AccountId == accountId);
        query = query.Where(x => x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false);

        return await query.Select(GetProjection()).ToListAsync(cancellationToken);
    }

    public async Task<DateTime?> GetFirstMovement(
        Domain.Engagement.Budget.Accounts.Accounts.ValueObjects.AccountId accountId, CancellationToken cancellationToken)
     => await _dbSet.Where(x => x.AccountId == accountId && x.IsDeleted == false)
            .OrderBy(x => x.Date)
            .Select(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<DateTime?> GetLastMovement(
        Domain.Engagement.Budget.Accounts.Accounts.ValueObjects.AccountId accountId, CancellationToken cancellationToken)
    => await _dbSet.Where(x => x.AccountId == accountId && x.IsDeleted == false)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<SearchOutput<FinancialMovementOutput>> SearchOutputAsync(SearchInputMovement input, CancellationToken cancellationToken)
    {
        List<Expression<Func<FinancialMovement, bool>>> where = [];

        where.Add(x => x.Date.Year == input.Year.Value && x.Date.Month == input.Month.Value);
        where.Add(x => x.IsDeleted == false);
        where.Add(x => x.AccountId == input.AccountId);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where.Add(x => x.Description.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase));
        }

        var items = base.GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .Select(GetProjection()).ToList();

        return Task.FromResult(new SearchOutput<FinancialMovementOutput>(input.Page, input.PerPage, total, items!));
    }

    private static Expression<Func<FinancialMovement, FinancialMovementOutput>> GetProjection()
    {
        return x => new FinancialMovementOutput()
        {
            Date = x.Date,
            Description = x.Description,
            Id = x.Id,
            PaymentMethod = new PaymentMethodOutput()
            {
                Id = x.PaymentMethodId,
                Name = x.PaymentMethod.Name,
                DeactivationDate = x.PaymentMethod.DeactivationDate,
                Description = x.PaymentMethod.Description,
                StartDate = x.PaymentMethod.StartDate
            },
            Status = new FinancialMovementStatusOutput(x.Status.Key, x.Status.Name),
            Type = new MovementTypeOutput(x.Type.Key, x.Type.Name),
            SubCategory = new SubCategoryOutput()
            {
                Name = x.SubCategory.Name,
                Description = x.SubCategory.Description,
                Id = x.SubCategory.Id,
                Category = new Application.Dto.Engagement.Budget.Categories.Response.CategoryOutput()
                {
                    Id = x.SubCategory.Category.Id,
                    Name = x.SubCategory.Category.Name,
                    Type = new Application.Dto.Engagement.Budget.Categories.Response.CategoryTypeOutput(
                        x.SubCategory.Category.Type.Key,
                        x.SubCategory.Category.Type.Name)
                },
            },
            Value = x.Value
        };
    }
}

using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
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

    public Task<ListSubCategoriesOutput> SearchAsync(SearchInputSubCategory input, CancellationToken cancellationToken)
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
            .Select(GetProjection())
            .OrderBy(x => x.Order);

        var items = query.ToList();

        return Task.FromResult(new ListSubCategoriesOutput(input.Page, input.PerPage, total, items!));
    }

    private static Expression<Func<AccountSubCategory, SubCategoryOutput>> GetProjection()
    {
        return x => new SubCategoryOutput()
        {
            Id = x.SubCategory.Id,
            Name = x.SubCategory.Name,
            Description = x.SubCategory.Description,
            StartDate = x.SubCategory.StartDate,
            DeactivationDate = x.SubCategory.DeactivationDate,
            Category = new CategoryOutput()
            {
                Id = x.SubCategory.Category.Id,
                Name = x.SubCategory.Category.Name,
                Description = x.SubCategory.Category.Description,
                StartDate = x.SubCategory.Category.StartDate,
                DeactivationDate = x.SubCategory.Category.DeactivationDate,
                Type = new CategoryTypeOutput(
                        x.SubCategory.Category.Type.Key,
                        x.SubCategory.Category.Type.Name)
            },
            DefaultPaymentMethod = new Application.Dto.Engagement.Budget.PaymentMethods.Responses.PaymentMethodOutput()
            {
                Id = x.SubCategory.DefaultPaymentMethod.Id,
                Name = x.SubCategory.DefaultPaymentMethod.Name,
                Description = x.SubCategory.DefaultPaymentMethod.Description,
                StartDate = x.SubCategory.DefaultPaymentMethod.StartDate,
                DeactivationDate = x.SubCategory.DefaultPaymentMethod.DeactivationDate,
            },
            Order = x.Order
        };
    }
}

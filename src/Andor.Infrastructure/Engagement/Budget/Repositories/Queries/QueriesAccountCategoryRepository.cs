﻿using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Andor.Infrastructure.Engagement.Budget.Repositories.Queries;

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

    public Task<ListCategoriesOutput> SearchAsync(SearchInputCategory input, CancellationToken cancellationToken)
    {
        List<Expression<Func<AccountCategory, bool>>> where = [];

        where.Add(x => x.AccountId == input.accountId);
        where.Add(x => x.Category.Type == input.Type);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where.Add(x => x.Category.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase));
        }

        var items = Extension.GetManyPaginated(
            _dbSet,
            loggedUserFilter,
            where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .Select(GetProjection())
            .OrderBy(x => x.Order)
            .ToList();

        return Task.FromResult(new ListCategoriesOutput(input.Page, input.PerPage, total, items!));
    }

    private static Expression<Func<AccountCategory, CategoryOutput>> GetProjection()
    {
        return x => new CategoryOutput()
        {
            Id = x.Category.Id,
            Name = x.Category.Name,
            Description = x.Category.Description,
            StartDate = x.Category.StartDate,
            DeactivationDate = x.Category.DeactivationDate,
            Type = new CategoryTypeOutput(
                        x.Category.Type.Key,
                        x.Category.Type.Name),
            Order = x.Order
        };
    }
}

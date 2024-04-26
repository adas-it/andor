using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public record SearchInputSubCategory(int Page,
    int PerPage,
    string? Search,
    string? OrderBy,
    SearchOrder Order,
    CategoryId? CategoryId,
    AccountId AccountId)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);

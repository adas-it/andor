using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;

public record SearchInputCategory(int Page,
    int PerPage,
    string? Search,
    string? OrderBy,
    SearchOrder Order,
    MovementType Type,
    AccountId accountId)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);

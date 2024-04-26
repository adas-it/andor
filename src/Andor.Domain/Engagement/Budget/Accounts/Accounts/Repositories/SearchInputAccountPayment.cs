using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public record SearchInputAccountPayment(int Page, int PerPage, string? Search, string? OrderBy,
    SearchOrder Order, MovementType Type, AccountId AccountId)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);

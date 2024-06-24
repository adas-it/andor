using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;

public record SearchInputMovement(int Page, int PerPage, string? Search, string? OrderBy,
    SearchOrder Order, MovementType Type, AccountId AccountId, Year Year, Month Month)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);

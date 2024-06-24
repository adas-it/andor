using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.Repositories;

public record SearchInputCashFlow(int Page, int PerPage, string? Search, string? OrderBy,
    SearchOrder Order, AccountId AccountId, Year Year, Month Month)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);

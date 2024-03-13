namespace Family.Budget.Domain.Entities.FinancialMovement.Repository;

using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public record SearchInputMovement(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order, AccountId AccountId, int Year, int Month)

    : SearchInput(Page, PerPage, Search, OrderBy, Order);

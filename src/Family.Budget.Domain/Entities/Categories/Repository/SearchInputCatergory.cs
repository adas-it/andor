namespace Family.Budget.Domain.Entities.Categories.Repository;

using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public record SearchInputCategory : SearchInput
{
    public SearchInputCategory(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order) : base(Page, PerPage, Search, OrderBy, Order)
    {
    }

    public MovementType Type { get; set; }
}

namespace Family.Budget.Domain.SeedWork.ShearchableRepository;

public record SearchInput(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order);
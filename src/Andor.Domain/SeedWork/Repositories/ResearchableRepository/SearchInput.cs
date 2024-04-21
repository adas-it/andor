namespace Andor.Domain.SeedWork.Repositories.ResearchableRepository;

public record SearchInput(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order);
namespace Andor.Domain.SeedWork.Repositories.ResearchableRepository;

public record SearchOutput<TAggregate>(
    int CurrentPage,
    int PerPage,
    int Total,
    ICollection<TAggregate> Items
);

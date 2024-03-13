namespace Andor.Domain.SeedWork.Repository.ISearchableRepository;

public record SearchOutput<TAggregate>(
    int CurrentPage,
    int PerPage,
    int Total,
    ICollection<TAggregate> Items
);

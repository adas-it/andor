namespace Andor.Foundation.Contracts.Results;

public record PaginatedListOutput<T>
{
    public int CurrentPage { get; init; }
    public int PerPage { get; init; }
    public int Total { get; init; }
    public List<T> Items { get; init; } = [];

    protected PaginatedListOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<T> items)
    {
        CurrentPage = page;
        PerPage = perPage;
        Total = total;
        Items = items.ToList();
    }
}

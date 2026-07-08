namespace Andor.Foundation.Contracts.Results;

public record PaginatedListOutput<T>
{
    public int CurrentPage { get; init; }
    public int PerPage { get; init; }
    public int Total { get; init; }
    public List<T> Items { get; init; } = [];
}

namespace Andor.Foundation.Application.Queries;

public record SearchInput
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public SearchOrder Order { get; set; }

    public void Normalize()
    {
        if (Page < 0)
            Page = 0;
        if (PerPage <= 0)
            PerPage = 10;
        if (Order == SearchOrder.Undefined)
            Order = SearchOrder.Asc;
    }
}

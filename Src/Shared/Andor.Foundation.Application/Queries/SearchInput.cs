namespace Andor.Foundation.Application.Queries;

public record SearchInput
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public SearchOrder Order { get; set; }

    public SearchInput()
    {
    }

    public SearchInput(int? page, int? perPage, string? search, string? orderBy, SearchOrder? order)
    {
        Page = page ?? 0;
        PerPage = perPage ?? 10;
        Search = search;
        OrderBy = orderBy;
        Order = order ?? SearchOrder.Asc;
    }

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

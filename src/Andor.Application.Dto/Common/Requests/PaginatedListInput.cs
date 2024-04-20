namespace Andor.Application.Dto.Common.Requests;

public record PaginatedListInput
{
    public PaginatedListInput()
    {
        Page = 1;
        PerPage = 10;
        Dir = SearchOrder.Asc;
    }

    public int Page { get; set; }
    public int PerPage { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public SearchOrder Dir { get; set; }
}
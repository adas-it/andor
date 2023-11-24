using Family.Budget.Application.Dto.Models.Response;

namespace Family.Budget.Application.Dto.Configurations.Responses;
public record ListConfigurationsOutput
    : PaginatedListOutput<ConfigurationOutput>
{
    public ListConfigurationsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<ConfigurationOutput> items)
        : base(page, perPage, total, items)
    {
    }
}

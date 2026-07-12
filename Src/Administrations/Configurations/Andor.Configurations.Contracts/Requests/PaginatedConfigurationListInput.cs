using Andor.Foundation.Contracts.Requests;

namespace Andor.Configurations.Contracts.Requests;

public record PaginatedConfigurationListInput : PaginatedListInput
{
    public List<int> States { get; set; }
}

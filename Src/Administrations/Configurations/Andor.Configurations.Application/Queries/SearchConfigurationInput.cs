using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Queries;

namespace Andor.Configurations.Application.Queries;

public record SearchConfigurationInput : SearchInput
{
    public ConfigurationState[] States { get; set; } = [];
}

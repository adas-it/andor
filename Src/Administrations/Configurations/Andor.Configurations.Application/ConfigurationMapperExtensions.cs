using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain;

namespace Andor.Configurations.Application;

internal static class ConfigurationMapperExtensions
{
    public static ConfigurationOutput? ToConfigurationOutput(this Configuration? config)
    {
        if (config == null) return null;

        return new ConfigurationOutput(config.Id, config.Name, config.Value, config.Description,
            config.StartDate, config.ExpireDate, new KeyValuePair<int, string>(config.State.Key, config.State.Name));
    }
}

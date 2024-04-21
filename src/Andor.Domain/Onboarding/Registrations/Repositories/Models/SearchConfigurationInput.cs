using Andor.Domain.Administrations.Configurations.ValueObjects;

namespace Andor.Domain.Onboarding.Registrations.Repositories.Models;

public record SearchConfigurationInput(string Name, ConfigurationState[] States);
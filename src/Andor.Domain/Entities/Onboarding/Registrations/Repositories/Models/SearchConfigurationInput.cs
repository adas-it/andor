using Andor.Domain.Entities.Admin.Configurations;

namespace Andor.Domain.Entities.Onboarding.Registrations.Repositories.Models;

public record SearchConfigurationInput(string Name, ConfigurationState[] States);
using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Admin.Configurations.ValueObjects;

namespace Andor.Domain.Entities.Onboarding.Registrations.Repositories.Models;

public record SearchConfigurationInput(string Name, ConfigurationState[] States);
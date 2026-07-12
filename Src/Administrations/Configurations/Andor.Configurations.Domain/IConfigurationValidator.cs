using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Domain;

public interface IConfigurationValidator : IDefaultValidator<Configuration, ConfigurationId>
{
    Task<List<Notification>> ValidateUpdateAsync(Configuration existing, string value, string description,
        DateTime startDate, DateTime? expireDate, CancellationToken cancellationToken);
}

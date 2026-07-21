using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;
using Value = Andor.Foundation.Domain.ValuesObjects.Value;

namespace Andor.Configurations.Domain.Tests.Configurations;

/// <summary>
/// Fixture for creating Configuration instances for testing purposes.
/// </summary>
internal static class ConfigurationFixture
{
    /// <summary>
    /// Creates a valid configuration starting now (Active state) with a default-success validator.
    /// </summary>
    public static async Task<(DomainResult result, Configuration? configuration)> CreateValidConfigurationAsync(
        Mock<IConfigurationValidator>? validatorMock = null,
        ConfigurationId? id = null,
        Name? name = null,
        Value? value = null,
        Description? description = null,
        DateTime? startDate = null,
        DateTime? expireDate = null,
        ConfigurationType? type = null,
        bool skipValidations = false,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var validator = validatorMock ?? CreateDefaultValidator();

        var result = await Configuration.NewAsync(
            id ?? ConfigurationId.New(),
            name ?? GeneralFixture.GetValidName(),
            value ?? GeneralFixture.GetValidValue(),
            description ?? GeneralFixture.GetValidDescription(),
            startDate ?? DateTime.UtcNow.AddMinutes(-5),
            expireDate,
            type ?? ConfigurationType.Generic,
            skipValidations,
            userId ?? Guid.NewGuid(),
            validator.Object,
            cancellationToken);

        return result;
    }

    public static Mock<IConfigurationValidator> CreateDefaultValidator()
    {
        var validatorMock = new Mock<IConfigurationValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
        _ = validatorMock
            .Setup(v => v.ValidateUpdateAsync(
                It.IsAny<Configuration>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
        return validatorMock;
    }

    public static Mock<IConfigurationValidator> CreateFailingValidator(List<Notification> notifications)
    {
        var validatorMock = new Mock<IConfigurationValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        return validatorMock;
    }
}

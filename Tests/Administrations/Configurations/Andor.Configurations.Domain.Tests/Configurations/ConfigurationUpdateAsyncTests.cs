using Andor.Configurations.Domain.Events;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Configurations.Domain.Tests.Configurations;

public class ConfigurationUpdateAsyncTests
{
    private readonly Mock<IConfigurationValidator> _validatorMock;

    public ConfigurationUpdateAsyncTests()
    {
        _validatorMock = ConfigurationFixture.CreateDefaultValidator();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateFieldsAndReturnSuccess()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(_validatorMock);
        var newValue = GeneralFixture.GetValidValue();
        var newDescription = GeneralFixture.GetValidDescription();
        var newStartDate = DateTime.UtcNow.AddMinutes(-10);
        var newExpireDate = DateTime.UtcNow.AddDays(30);

        // Act
        var result = await configuration!.UpdateAsync(
            newValue, newDescription, newStartDate, newExpireDate, Guid.NewGuid(),
            _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newValue.Value, configuration.Value.Value);
        Assert.Equal(newDescription.Value, configuration.Description.Value);
        Assert.Equal(newStartDate, configuration.StartDate);
        Assert.Equal(newExpireDate, configuration.ExpireDate);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldRaiseConfigurationUpdatedDomainEvent()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(_validatorMock);
        var userId = Guid.NewGuid();

        // Act
        var result = await configuration!.UpdateAsync(
            GeneralFixture.GetValidValue(), GeneralFixture.GetValidDescription(),
            DateTime.UtcNow.AddMinutes(-10), null, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var domainEvent = Assert.Single(configuration.Events.OfType<ConfigurationUpdated>());
        Assert.Equal(userId, domainEvent.UserId);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidatorReturnsFailure_ShouldNotChangeFields()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(_validatorMock);
        var originalValue = configuration!.Value;
        var originalStartDate = configuration.StartDate;
        configuration.ClearEvents();

        var notification = new Notification("Value", "invalid", CommonErrorCodes.Validation);
        _ = _validatorMock
            .Setup(v => v.ValidateUpdateAsync(
                It.IsAny<Configuration>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([notification]);

        // Act
        var result = await configuration.UpdateAsync(
            GeneralFixture.GetValidValue(), GeneralFixture.GetValidDescription(),
            DateTime.UtcNow.AddDays(1), null, Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(originalValue, configuration.Value);
        Assert.Equal(originalStartDate, configuration.StartDate);
        Assert.Empty(configuration.Events);
    }
}

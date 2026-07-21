using Andor.Configurations.Domain.Errors;
using Andor.Configurations.Domain.Events;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;

namespace Andor.Configurations.Domain.Tests.Configurations;

public class ConfigurationNewAsyncTests
{
    private readonly Mock<IConfigurationValidator> _validatorMock;

    public ConfigurationNewAsyncTests()
    {
        _validatorMock = ConfigurationFixture.CreateDefaultValidator();
    }

    #region NewAsync - Success Cases

    [Fact]
    public async Task NewAsync_WithValidData_ShouldCreateConfigurationSuccessfully()
    {
        // Arrange
        var id = ConfigurationId.New();
        var name = GeneralFixture.GetValidName();
        var value = GeneralFixture.GetValidValue();
        var description = GeneralFixture.GetValidDescription();
        var startDate = DateTime.UtcNow.AddMinutes(-5);
        var userId = Guid.NewGuid();

        // Act
        var (result, configuration) = await Configuration.NewAsync(
            id, name, value, description, startDate, null, ConfigurationType.Generic,
            skipValidations: false, userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration);
        Assert.Equal(id, configuration.Id);
        Assert.Equal(value.Value, configuration.Value.Value);
        Assert.Equal(description.Value, configuration.Description.Value);
        Assert.Equal(startDate, configuration.StartDate);
        Assert.Null(configuration.ExpireDate);
        Assert.Equal(ConfigurationType.Generic, configuration.Type);
        Assert.False(configuration.IsDeleted);
    }

    [Fact]
    public async Task NewAsync_ShouldLowercaseName()
    {
        // Arrange
        var id = ConfigurationId.New();
        Name name = "MixedCaseName";

        // Act
        var (result, configuration) = await Configuration.NewAsync(
            id, name, GeneralFixture.GetValidValue(), GeneralFixture.GetValidDescription(),
            DateTime.UtcNow.AddMinutes(-5), null, ConfigurationType.Generic,
            skipValidations: false, Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration);
        Assert.Equal("mixedcasename", configuration.Name.Value);
    }

    [Fact]
    public async Task NewAsync_ShouldRaiseConfigurationCreatedDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (result, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, userId: userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration);
        _ = Assert.Single(configuration.Events);

        var domainEvent = Assert.IsType<ConfigurationCreated>(configuration.Events.First());
        Assert.Equal(userId, domainEvent.UserId);
        Assert.Equal(configuration.Name.Value, domainEvent.Name);
    }

    [Fact]
    public async Task NewAsync_WhenSkipValidationsIsTrue_ShouldNotCallValidator()
    {
        // Act
        var (result, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, skipValidations: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration);
        Assert.Contains(result.Infos, i => i.Error.Equals(ConfigurationsErrorCodes.SkippedValidations));
        _validatorMock.Verify(
            v => v.ValidateCreationAsync(It.IsAny<Configuration>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task NewAsync_WhenSkipValidationsIsTrue_ShouldStillRaiseDomainEvent()
    {
        // Act
        var (result, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, skipValidations: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration);
        _ = Assert.Single(configuration.Events);
        _ = Assert.IsType<ConfigurationCreated>(configuration.Events.First());
    }

    #endregion

    #region NewAsync - Validation Failure Cases

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var notification = new Notification("Value", "invalid", CommonErrorCodes.Validation);
        var failingValidator = ConfigurationFixture.CreateFailingValidator([notification]);

        // Act
        var (result, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(failingValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(configuration);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var notification = new Notification("Value", "invalid", CommonErrorCodes.Validation);
        var failingValidator = ConfigurationFixture.CreateFailingValidator([notification]);

        // Act
        var (result, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(failingValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(configuration);
        Assert.Empty(configuration.Events);
    }

    #endregion
}

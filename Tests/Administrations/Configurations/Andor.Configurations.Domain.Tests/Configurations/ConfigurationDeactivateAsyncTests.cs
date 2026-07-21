using Andor.Configurations.Domain.Errors;
using Andor.Configurations.Domain.Events;
using Andor.Configurations.Domain.ValueObjects;
using Moq;

namespace Andor.Configurations.Domain.Tests.Configurations;

public class ConfigurationDeactivateAsyncTests
{
    private readonly Mock<IConfigurationValidator> _validatorMock;

    public ConfigurationDeactivateAsyncTests()
    {
        _validatorMock = ConfigurationFixture.CreateDefaultValidator();
    }

    [Fact]
    public async Task DeactivateAsync_WhenActive_ShouldSetExpireDateAndReturnSuccess()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(-1));
        Assert.Equal(ConfigurationState.Active, configuration!.State);

        // Act
        var result = await configuration.DeactivateAsync(Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(configuration.ExpireDate);
        Assert.True(configuration.ExpireDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task DeactivateAsync_WhenActive_ShouldRaiseConfigurationDeactivatedDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(-1));

        // Act
        var result = await configuration!.DeactivateAsync(userId, _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var domainEvent = Assert.Single(configuration.Events.OfType<ConfigurationDeactivated>());
        Assert.Equal(userId, domainEvent.UserId);
    }

    [Fact]
    public async Task DeactivateAsync_WhenAwaiting_ShouldReturnFailure()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(1));
        Assert.Equal(ConfigurationState.Awaiting, configuration!.State);

        // Act
        var result = await configuration.DeactivateAsync(Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,
            e => e.Error.Equals(ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedAwaiting));
        Assert.Null(configuration.ExpireDate);
    }

    [Fact]
    public async Task DeactivateAsync_WhenExpired_ShouldReturnFailure()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(-2), expireDate: DateTime.UtcNow.AddDays(-1));
        Assert.Equal(ConfigurationState.Expired, configuration!.State);

        // Act
        var result = await configuration.DeactivateAsync(Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,
            e => e.Error.Equals(ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedExpired));
    }

    [Fact]
    public async Task DeactivateAsync_WhenDeleted_ShouldReturnFailure()
    {
        // Arrange - awaiting configuration deleted via DeleteAsync to reach Deleted state
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(1));
        _ = await configuration!.DeleteAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Equal(ConfigurationState.Deleted, configuration.State);

        // Act
        var result = await configuration.DeactivateAsync(Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,
            e => e.Error.Equals(ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedDeleted));
    }
}

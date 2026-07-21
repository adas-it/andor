using Andor.Configurations.Domain.Errors;
using Andor.Configurations.Domain.Events;
using Andor.Configurations.Domain.ValueObjects;
using Moq;

namespace Andor.Configurations.Domain.Tests.Configurations;

public class ConfigurationDeleteAsyncTests
{
    private readonly Mock<IConfigurationValidator> _validatorMock;

    public ConfigurationDeleteAsyncTests()
    {
        _validatorMock = ConfigurationFixture.CreateDefaultValidator();
    }

    [Fact]
    public async Task DeleteAsync_WhenAwaiting_ShouldMarkAsDeletedAndReturnSuccess()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(1));
        Assert.Equal(ConfigurationState.Awaiting, configuration!.State);

        // Act
        var result = await configuration.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(configuration.IsDeleted);
        Assert.NotNull(configuration.DeletedDate);
        Assert.Equal(ConfigurationState.Deleted, configuration.State);
    }

    [Fact]
    public async Task DeleteAsync_WhenAwaiting_ShouldRaiseConfigurationDeletedDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(1));

        // Act
        var result = await configuration!.DeleteAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var domainEvent = Assert.Single(configuration.Events.OfType<ConfigurationDeleted>());
        Assert.Equal(userId, domainEvent.UserId);
    }

    [Fact]
    public async Task DeleteAsync_WhenActive_ShouldReturnFailureAndNotDelete()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(-1));
        Assert.Equal(ConfigurationState.Active, configuration!.State);

        // Act
        var result = await configuration.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(configuration.IsDeleted);
        Assert.Contains(result.Errors,
            e => e.Error.Equals(ConfigurationsErrorCodes.ErrorOnDeleteConfigurationNotAllowedDeleteActive));
    }

    [Fact]
    public async Task DeleteAsync_WhenExpired_ShouldReturnSuccessButNotDelete()
    {
        // Arrange - an expired configuration is silently ignored (warning only), not an error
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(-2), expireDate: DateTime.UtcNow.AddDays(-1));
        Assert.Equal(ConfigurationState.Expired, configuration!.State);

        // Act
        var result = await configuration.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(configuration.IsDeleted);
        Assert.Contains(result.Warnings,
            e => e.Error.Equals(ConfigurationsErrorCodes.ErrorOnDeleteConfigurationNotAllowedDeleteExpired));
    }

    [Fact]
    public async Task DeleteAsync_WhenAlreadyDeleted_ShouldReturnSuccessAndStayIdempotent()
    {
        // Arrange
        var (_, configuration) = await ConfigurationFixture.CreateValidConfigurationAsync(
            _validatorMock, startDate: DateTime.UtcNow.AddDays(1));
        _ = await configuration!.DeleteAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.True(configuration.IsDeleted);
        var deletedDate = configuration.DeletedDate;

        // Act - delete again
        var result = await configuration.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(configuration.IsDeleted);
        Assert.Equal(deletedDate, configuration.DeletedDate);
        Assert.Single(configuration.Events.OfType<ConfigurationDeleted>());
    }
}

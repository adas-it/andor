using Andor.Configurations.Domain.ValueObjects;

namespace Andor.Configurations.Domain.Tests.Configurations;

public class ConfigurationCalculateStateTests
{
    [Fact]
    public void CalculateState_WhenIsDeleted_ShouldReturnDeleted()
    {
        var state = Configuration.CalculateState(
            isDeleted: true,
            startDate: DateTime.UtcNow.AddDays(-1),
            expireDate: DateTime.UtcNow.AddDays(1));

        Assert.Equal(ConfigurationState.Deleted, state);
    }

    [Fact]
    public void CalculateState_WhenStartDateInFuture_ShouldReturnAwaiting()
    {
        var state = Configuration.CalculateState(
            isDeleted: false,
            startDate: DateTime.UtcNow.AddDays(1),
            expireDate: null);

        Assert.Equal(ConfigurationState.Awaiting, state);
    }

    [Fact]
    public void CalculateState_WhenStartDateInPastAndNoExpireDate_ShouldReturnActive()
    {
        var state = Configuration.CalculateState(
            isDeleted: false,
            startDate: DateTime.UtcNow.AddDays(-1),
            expireDate: null);

        Assert.Equal(ConfigurationState.Active, state);
    }

    [Fact]
    public void CalculateState_WhenStartDateInPastAndExpireDateInFuture_ShouldReturnActive()
    {
        var state = Configuration.CalculateState(
            isDeleted: false,
            startDate: DateTime.UtcNow.AddDays(-1),
            expireDate: DateTime.UtcNow.AddDays(1));

        Assert.Equal(ConfigurationState.Active, state);
    }

    [Fact]
    public void CalculateState_WhenExpireDateInPast_ShouldReturnExpired()
    {
        var state = Configuration.CalculateState(
            isDeleted: false,
            startDate: DateTime.UtcNow.AddDays(-2),
            expireDate: DateTime.UtcNow.AddDays(-1));

        Assert.Equal(ConfigurationState.Expired, state);
    }

    [Fact]
    public void CalculateState_IsDeleted_TakesPrecedenceOverExpired()
    {
        var state = Configuration.CalculateState(
            isDeleted: true,
            startDate: DateTime.UtcNow.AddDays(-2),
            expireDate: DateTime.UtcNow.AddDays(-1));

        Assert.Equal(ConfigurationState.Deleted, state);
    }
}

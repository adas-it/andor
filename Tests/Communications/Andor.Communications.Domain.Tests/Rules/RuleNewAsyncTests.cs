using Andor.Communications.Domain.Errors;
using Andor.Communications.Domain.Events;
using Andor.Communications.Domain.ValueObjects;
using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;
using RuleType = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Domain.Tests.Rules;

public class RuleNewAsyncTests
{
    private readonly Mock<IRuleValidator> _validatorMock;

    public RuleNewAsyncTests()
    {
        _validatorMock = RuleFixture.CreateDefaultValidator();
    }

    #region NewAsync - Success Cases

    [Fact]
    public async Task NewAsync_WithValidData_ShouldCreateRuleSuccessfully()
    {
        // Arrange
        var id = RuleId.New();
        var name = GeneralFixture.GetValidName().Value!;

        // Act
        var (result, rule) = await Rule.NewAsync(
            id, name, RuleType.Marketing, [], skipValidations: false,
            Guid.NewGuid(), _validatorMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(rule);
        Assert.Equal(id, rule.Id);
        Assert.Equal(name, rule.Name.Value);
        Assert.Equal(RuleType.Marketing, rule.Type);
        Assert.Empty(rule.Templates);
    }

    [Fact]
    public async Task NewAsync_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var (result, rule) = await RuleFixture.CreateValidRuleAsync(_validatorMock);
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(rule);
        Assert.InRange(rule.CreatedAt, before, after);
    }

    [Fact]
    public async Task NewAsync_ShouldRaiseRuleCreatedDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (result, rule) = await RuleFixture.CreateValidRuleAsync(_validatorMock, userId: userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(rule);
        var domainEvent = Assert.Single(rule.Events);
        var ruleCreated = Assert.IsType<RuleCreated>(domainEvent);
        Assert.Equal(userId, ruleCreated.UserId);
        Assert.Equal(rule.Name.Value, ruleCreated.Name);
    }

    [Fact]
    public async Task NewAsync_WhenSkipValidationsIsTrue_ShouldNotCallValidator()
    {
        // Act
        var (result, rule) = await RuleFixture.CreateValidRuleAsync(_validatorMock, skipValidations: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(rule);
        Assert.Contains(result.Infos, i => i.Error.Equals(CommunicationsErrorCodes.SkippedValidations));
        _validatorMock.Verify(
            v => v.ValidateCreationAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region NewAsync - Validation Failure Cases

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var notification = new Notification("Name", "invalid", CommonErrorCodes.Validation);
        var failingValidator = RuleFixture.CreateFailingValidator([notification]);

        // Act
        var (result, rule) = await RuleFixture.CreateValidRuleAsync(failingValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task NewAsync_WhenValidationFails_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var notification = new Notification("Name", "invalid", CommonErrorCodes.Validation);
        var failingValidator = RuleFixture.CreateFailingValidator([notification]);

        // Act
        var (result, rule) = await RuleFixture.CreateValidRuleAsync(failingValidator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(rule);
        Assert.Empty(rule.Events);
    }

    #endregion
}

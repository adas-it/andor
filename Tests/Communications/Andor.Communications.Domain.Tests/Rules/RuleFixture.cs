using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;
using Moq;
using RuleType = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Domain.Tests.Rules;

/// <summary>
/// Fixture for creating Rule instances for testing purposes.
/// </summary>
internal static class RuleFixture
{
    public static async Task<(DomainResult result, Rule? rule)> CreateValidRuleAsync(
        Mock<IRuleValidator>? validatorMock = null,
        RuleId? id = null,
        string? name = null,
        RuleType? type = null,
        List<Template>? templates = null,
        bool skipValidations = false,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var validator = validatorMock ?? CreateDefaultValidator();

        var result = await Rule.NewAsync(
            id ?? RuleId.New(),
            name ?? GeneralFixture.GetValidName().Value!,
            type ?? RuleType.Information,
            templates ?? [],
            skipValidations,
            userId ?? Guid.NewGuid(),
            validator.Object,
            cancellationToken);

        return result;
    }

    public static Mock<IRuleValidator> CreateDefaultValidator()
    {
        var validatorMock = new Mock<IRuleValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());
        return validatorMock;
    }

    public static Mock<IRuleValidator> CreateFailingValidator(List<Notification> notifications)
    {
        var validatorMock = new Mock<IRuleValidator>();
        _ = validatorMock
            .Setup(v => v.ValidateCreationAsync(It.IsAny<Rule>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        return validatorMock;
    }
}

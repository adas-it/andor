using Andor.Communications.Domain.ValueObjects;

namespace Andor.Communications.Domain.Tests.Templates;

public class TemplateNewTests
{
    [Fact]
    public async Task New_WithValidData_ShouldCreateTemplateSuccessfully()
    {
        // Arrange
        var rule = await TemplateFixture.CreateOwnerRuleAsync();

        // Act
        var (result, template) = TemplateFixture.CreateValidTemplate(
            rule, value: "Hello!", contentLanguage: "pt-BR", title: "Welcome", partner: Partner.SendGrid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(template);
        Assert.Equal("Hello!", template.Value);
        Assert.Equal("pt-BR", template.ContentLanguage);
        Assert.Equal("Welcome", template.Title);
        Assert.Equal(Partner.SendGrid, template.Partner);
        Assert.Equal(rule.Id, template.RuleId);
        Assert.Same(rule, template.Rule);
    }

    [Fact]
    public async Task New_WithEmptyTitle_ShouldReturnFailure()
    {
        // Arrange
        var rule = await TemplateFixture.CreateOwnerRuleAsync();

        // Act
        var (result, template) = TemplateFixture.CreateValidTemplate(rule, title: "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(template);
    }

    [Fact]
    public async Task New_WithTitleTooShort_ShouldReturnFailure()
    {
        // Arrange
        var rule = await TemplateFixture.CreateOwnerRuleAsync();

        // Act
        var (result, template) = TemplateFixture.CreateValidTemplate(rule, title: "A");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(template);
    }

    [Fact]
    public async Task New_WithTitleTooLong_ShouldReturnFailure()
    {
        // Arrange
        var rule = await TemplateFixture.CreateOwnerRuleAsync();
        var longTitle = new string('A', 51);

        // Act
        var (result, template) = TemplateFixture.CreateValidTemplate(rule, title: longTitle);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(template);
    }

    [Fact]
    public async Task New_WithTitleAtBoundaries_ShouldCreateSuccessfully()
    {
        // Arrange
        var rule = await TemplateFixture.CreateOwnerRuleAsync();

        // Act
        var (resultMin, templateMin) = TemplateFixture.CreateValidTemplate(rule, title: "AB");
        var (resultMax, templateMax) = TemplateFixture.CreateValidTemplate(rule, title: new string('A', 50));

        // Assert
        Assert.True(resultMin.IsSuccess);
        Assert.NotNull(templateMin);
        Assert.True(resultMax.IsSuccess);
        Assert.NotNull(templateMax);
    }
}

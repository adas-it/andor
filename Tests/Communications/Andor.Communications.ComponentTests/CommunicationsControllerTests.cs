using System.Net;
using System.Net.Http.Json;
using Andor.ComponentTests.Common;
using Andor.Communications.Contracts.Requests;
using Andor.Communications.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Communications.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Communications.RestApi.CommunicationsController"/>.
/// The in-house partner path (Partner=1) is exercised, with the real SMTP client swapped for
/// <see cref="FakeSmtp"/> by <see cref="CommunicationsApiFactory"/>.
/// </summary>
public sealed class CommunicationsControllerTests : IClassFixture<CommunicationsApiFactory>
{
    private const int TypeInformation = 1;
    private const int PartnerInHouse = 1;

    private readonly CommunicationsApiFactory _factory;

    public CommunicationsControllerTests(CommunicationsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateRuleAsync_ReturnsCreatedRule()
    {
        using var client = _factory.CreateAuthenticatedClient();

        var input = new CreateRuleInput("welcome-email", TypeInformation,
            [new RuleTemplateInput("Hello {{name}}", "pt-BR", "welcome-title", PartnerInHouse)], false);

        var response = await client.PostAsJsonAsync("v1/Communications/rules", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<RuleOutput>>(ComponentTestJson.Options);
        body!.Errors.Should().BeEmpty();
        body.Data.Should().NotBeNull();
        body.Data!.Name.Should().Be("welcome-email");
    }

    [Fact]
    public async Task CreateRuleAsync_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        client.SetAnonymous();

        var input = new CreateRuleInput("welcome-email", TypeInformation,
            [new RuleTemplateInput("Hello {{name}}", "pt-BR", "welcome-title", PartnerInHouse)], false);

        var response = await client.PostAsJsonAsync("v1/Communications/rules", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendNotificationAsync_DispatchesThroughFakeSmtp()
    {
        using var client = _factory.CreateAuthenticatedClient();

        var ruleInput = new CreateRuleInput("password-reset", TypeInformation,
            [new RuleTemplateInput("Your code is {{code}}", "pt-BR", "reset-title", PartnerInHouse)], false);

        var ruleResponse = await client.PostAsJsonAsync("v1/Communications/rules", ruleInput, ComponentTestJson.Options);
        var rule = await ruleResponse.Content.ReadFromJsonAsync<DefaultResponse<RuleOutput>>(ComponentTestJson.Options);

        var recipient = $"user-{Guid.NewGuid():N}@example.com";
        var notificationInput = new SendNotificationInput(rule!.Data!.Id, recipient, "Reset your password",
            "reset-title", new Dictionary<string, string> { ["{{code}}"] = "123456" });

        var response = await client.PostAsJsonAsync("v1/Communications/notifications", notificationInput, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _factory.Smtp.SentMessages.Should().ContainSingle(m =>
            m.RecipientEmail == recipient && m.Body.Contains("123456"));
    }
    // NOTE: sending a notification for a RuleId that was never created is intentionally not
    // covered here. RuleActor (like every actor in this Manager/Actor/Stash pattern - see
    // ConfigurationActor, AccountActor, SignupActor) never replies for a command routed to a
    // freshly-created child whose PreLoad finds nothing: the child stays in `Loading` forever,
    // stashing the command indefinitely instead of answering "not found". The command handler's
    // Ask call has no built-in timeout in that path, so this is a genuine hang, not just a slow
    // response - reproducing it here would just make the suite flaky/slow. Worth fixing at the
    // actor-framework level (reply not-found from Loading instead of stashing) across all four
    // modules that share this pattern.
}

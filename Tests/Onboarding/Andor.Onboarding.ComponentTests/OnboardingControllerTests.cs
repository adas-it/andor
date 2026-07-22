using System.Net;
using System.Net.Http.Json;
using Andor.ComponentTests.Common;
using Andor.Onboarding.Contracts.Requests;
using Andor.Onboarding.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Onboarding.RestApi.OnboardingController"/>: the one
/// public, unauthenticated controller in the system (pre-login signup flow).
/// </summary>
public sealed class OnboardingControllerTests : IClassFixture<OnboardingApiFactory>
{
    private readonly OnboardingApiFactory _factory;

    public OnboardingControllerTests(OnboardingApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task StartAsync_WithNewEmail_Succeeds()
    {
        using var client = _factory.CreateClient();

        var input = new StartSignupInput("Ada Lovelace", $"ada-{Guid.NewGuid():N}@example.com");
        var response = await client.PostAsJsonAsync("v1/Onboarding/start", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task VerifyAsync_WithCorrectCode_Succeeds()
    {
        using var client = _factory.CreateClient();
        var email = $"grace-{Guid.NewGuid():N}@example.com";

        var startResponse = await client.PostAsJsonAsync("v1/Onboarding/start",
            new StartSignupInput("Grace Hopper", email), ComponentTestJson.Options);
        startResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var code = await GetVerificationCodeAsync(email);

        var response = await client.PostAsJsonAsync("v1/Onboarding/verify",
            new VerifySignupInput(email, code, "S3curePassword!"), ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task VerifyAsync_WithWrongCode_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var email = $"katherine-{Guid.NewGuid():N}@example.com";

        await client.PostAsJsonAsync("v1/Onboarding/start",
            new StartSignupInput("Katherine Johnson", email), ComponentTestJson.Options);

        var response = await client.PostAsJsonAsync("v1/Onboarding/verify",
            new VerifySignupInput(email, "0000000000", "S3curePassword!"), ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyAsync_ForUnknownEmail_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("v1/Onboarding/verify",
            new VerifySignupInput($"missing-{Guid.NewGuid():N}@example.com", "0000000000", "S3curePassword!"),
            ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<string> GetVerificationCodeAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandsSignupRequestRepository>();
        var signupRequest = await repository.GetByEmailAsync(email, CancellationToken.None);
        return signupRequest!.VerificationCode;
    }
}

using System.Net;
using System.Net.Http.Json;
using Andor.Assets.Contracts;
using Andor.ComponentTests.Common;
using Andor.Foundation.Contracts.Results;

namespace Andor.Assets.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Assets.RestApi.AssetsController"/>.
/// </summary>
public sealed class AssetsControllerTests : IClassFixture<AssetsApiFactory>
{
    private readonly AssetsApiFactory _factory;

    public AssetsControllerTests(AssetsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateArea_ReturnsCreatedArea()
    {
        using var client = _factory.CreateAuthenticatedClient();

        var input = new AreaInput("Renda Fixa " + Guid.NewGuid().ToString("N"));
        var response = await client.PostAsJsonAsync("v1/Assets", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AreaOutput>>(ComponentTestJson.Options);
        body!.Errors.Should().BeEmpty();
        body.Data.Should().NotBeNull();
        body.Data!.Name.Should().Be(input.Name);
    }

    [Fact]
    public async Task CreateArea_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        client.SetAnonymous();

        var input = new AreaInput("Renda Fixa " + Guid.NewGuid().ToString("N"));
        var response = await client.PostAsJsonAsync("v1/Assets", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

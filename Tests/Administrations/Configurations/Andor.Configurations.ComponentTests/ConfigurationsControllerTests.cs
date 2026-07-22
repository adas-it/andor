using System.Net;
using System.Net.Http.Json;
using Andor.ComponentTests.Common;
using Andor.Configurations.Contracts.Requests;
using Andor.Configurations.Contracts.Responses;
using Andor.Foundation.Contracts.Results;

namespace Andor.Configurations.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Configurations.RestApi.ConfigurationsController"/>: real
/// HTTP calls through the module's actual ASP.NET Core pipeline (routing, authorization, Akka
/// actors, EF Core) with only the database and message bus swapped for test doubles.
/// </summary>
public sealed class ConfigurationsControllerTests : IClassFixture<ConfigurationsApiFactory>
{
    private static readonly TestUser AdminSenior = new(Guid.NewGuid(), "Administrador Senior");
    private static readonly TestUser Admin = new(Guid.NewGuid(), "Administrador");
    private static readonly TestUser NoPermissions = new(Guid.NewGuid(), "User");

    private readonly ConfigurationsApiFactory _factory;

    public ConfigurationsControllerTests(ConfigurationsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAsync_WithPermittedGroup_ReturnsCreatedConfiguration()
    {
        using var client = _factory.CreateAuthenticatedClient(AdminSenior);
        var name = UniqueName();

        var response = await CreateConfigurationAsync(client, name, DateTime.UtcNow, null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<ConfigurationOutput>>(ComponentTestJson.Options);

        body.Should().NotBeNull();
        body!.Errors.Should().BeEmpty();
        body.Data.Should().NotBeNull();
        body.Data!.Name.Should().Be(name);
    }

    [Fact]
    public async Task CreateAsync_WithoutCreatePermission_ReturnsBadRequest()
    {
        using var client = _factory.CreateAuthenticatedClient(Admin);

        var response = await CreateConfigurationAsync(client, UniqueName(), DateTime.UtcNow, null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        client.SetAnonymous();

        var response = await CreateConfigurationAsync(client, UniqueName(), DateTime.UtcNow, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetByIdAsync_WithReadPermission_ReturnsConfiguration()
    {
        using var creator = _factory.CreateAuthenticatedClient(AdminSenior);
        var created = await CreateAndReadConfigurationAsync(creator, UniqueName(), DateTime.UtcNow, null);

        using var reader = _factory.CreateAuthenticatedClient(Admin);
        var response = await reader.GetAsync($"v1/Configurations/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<ConfigurationOutput>>(ComponentTestJson.Options);
        body!.Data!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithoutReadPermission_ReturnsBadRequest()
    {
        using var creator = _factory.CreateAuthenticatedClient(AdminSenior);
        var created = await CreateAndReadConfigurationAsync(creator, UniqueName(), DateTime.UtcNow, null);

        using var reader = _factory.CreateAuthenticatedClient(NoPermissions);
        var response = await reader.GetAsync($"v1/Configurations/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchAsync_IncludesNewlyCreatedConfiguration()
    {
        using var client = _factory.CreateAuthenticatedClient(AdminSenior);
        var name = UniqueName();
        _ = await CreateAndReadConfigurationAsync(client, name, DateTime.UtcNow, null);

        var response = await client.GetAsync($"v1/Configurations/Search?search={name}&page=0&perPage=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<DefaultResponse<PaginatedListOutput<ConfigurationOutput>>>(ComponentTestJson.Options);

        body!.Data!.Items.Should().ContainSingle(x => x.Name == name);
    }

    [Fact]
    public async Task ChangeAsync_UpdatesValueAndDescription()
    {
        using var client = _factory.CreateAuthenticatedClient(AdminSenior);
        var created = await CreateAndReadConfigurationAsync(client, UniqueName(), DateTime.UtcNow.AddMinutes(5), null);

        var change = new ChangeConfigurationInput(created.Name, "new-value", "new description", DateTime.UtcNow.AddMinutes(5), null);
        var response = await client.PutAsJsonAsync($"v1/Configurations/{created.Id}", change, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<ConfigurationOutput>>(ComponentTestJson.Options);
        body!.Data!.Value.Should().Be("new-value");
        body.Data!.Description.Should().Be("new description");
    }

    [Fact]
    public async Task DeactivateAsync_OnActiveConfiguration_ExpiresItImmediately()
    {
        using var client = _factory.CreateAuthenticatedClient(AdminSenior);
        var created = await CreateAndReadConfigurationAsync(client, UniqueName(), DateTime.UtcNow, null);

        var response = await client.PatchAsync($"v1/Configurations/{created.Id}/deactivate",
            JsonContent.Create<object?>(null, options: ComponentTestJson.Options));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterResponse = await client.GetAsync($"v1/Configurations/{created.Id}");
        var afterBody = await afterResponse.Content.ReadFromJsonAsync<DefaultResponse<ConfigurationOutput>>(ComponentTestJson.Options);

        afterBody!.Data!.ExpireDate.Should().NotBeNull();
        afterBody.Data!.ExpireDate!.Value.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task DeleteAsync_OnAwaitingConfiguration_RemovesItFromSearch()
    {
        using var client = _factory.CreateAuthenticatedClient(AdminSenior);
        var name = UniqueName();
        var created = await CreateAndReadConfigurationAsync(client, name, DateTime.UtcNow.AddMinutes(10), null);

        var deleteResponse = await client.DeleteAsync($"v1/Configurations/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var searchResponse = await client.GetAsync($"v1/Configurations/Search?search={name}&page=0&perPage=10");
        var searchBody = await searchResponse.Content
            .ReadFromJsonAsync<DefaultResponse<PaginatedListOutput<ConfigurationOutput>>>(ComponentTestJson.Options);

        searchBody!.Data!.Items.Should().NotContain(x => x.Id == created.Id);
    }

    private static async Task<HttpResponseMessage> CreateConfigurationAsync(HttpClient client, string name,
        DateTime startDate, DateTime? expireDate)
    {
        var input = new CreateConfigurationInput(name, "initial-value", "initial description", startDate, expireDate, false);
        return await client.PostAsJsonAsync("v1/Configurations", input, ComponentTestJson.Options);
    }

    private static async Task<ConfigurationOutput> CreateAndReadConfigurationAsync(HttpClient client, string name,
        DateTime startDate, DateTime? expireDate)
    {
        var response = await CreateConfigurationAsync(client, name, startDate, expireDate);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<ConfigurationOutput>>(ComponentTestJson.Options);
        return body!.Data!;
    }

    private static string UniqueName() => $"cfg-{Guid.NewGuid():N}";
}

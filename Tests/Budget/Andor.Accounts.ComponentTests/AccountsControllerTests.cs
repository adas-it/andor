using System.Net;
using System.Net.Http.Json;
using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.ComponentTests.Common;
using Andor.Foundation.Contracts.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Accounts.RestApi.AccountsController"/>. The BRL currency
/// used below is seeded for real by the module's own Program.cs (SeedDefaultCurrencyAsync) as
/// part of building the test host, against the same InMemory database the test hits.
/// </summary>
public sealed class AccountsControllerTests : IClassFixture<AccountsApiFactory>
{
    private readonly AccountsApiFactory _factory;

    public AccountsControllerTests(AccountsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedAccount()
    {
        var currencyId = await GetSeededBrlCurrencyIdAsync();
        using var client = _factory.CreateAuthenticatedClient();

        var input = new AccountInput("My Account", currencyId.ToString());
        var response = await client.PostAsJsonAsync("v1/Accounts", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        body!.Errors.Should().BeEmpty();
        body.Data.Should().NotBeNull();
        body.Data!.Name.Should().Be("My Account");
        body.Data!.CurrencyId.Should().Be(currencyId.ToString());
    }

    [Fact]
    public async Task CreateAsync_WithUnknownCurrency_ReturnsBadRequest()
    {
        using var client = _factory.CreateAuthenticatedClient();

        var input = new AccountInput("My Account", Guid.NewGuid().ToString());
        var response = await client.PostAsJsonAsync("v1/Accounts", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        client.SetAnonymous();

        var input = new AccountInput("My Account", Guid.NewGuid().ToString());
        var response = await client.PostAsJsonAsync("v1/Accounts", input, ComponentTestJson.Options);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCreatedAccount()
    {
        var currencyId = await GetSeededBrlCurrencyIdAsync();
        using var client = _factory.CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("v1/Accounts",
            new AccountInput("Lookup Account", currencyId.ToString()), ComponentTestJson.Options);
        var created = await createResponse.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);

        var response = await client.GetAsync($"v1/Accounts/{created!.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        body!.Data!.Id.Should().Be(created.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNoContent()
    {
        using var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync($"v1/Accounts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> GetSeededBrlCurrencyIdAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();
        var currency = await repository.GetByIsoAsync("BRL", CancellationToken.None);
        return currency!.Id;
    }
}

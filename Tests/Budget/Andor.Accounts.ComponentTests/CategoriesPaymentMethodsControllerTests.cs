using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.Categories;
using Andor.Accounts.Contracts.PaymentMethods;
using Andor.Accounts.Contracts.SubCategories;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.MovementTypes;
using Andor.ComponentTests.Common;
using Andor.Foundation.Contracts.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.ComponentTests;

/// <summary>
/// Component tests for the write endpoints on <see cref="Andor.Accounts.RestApi.AccountCategoriesController"/>,
/// <see cref="Andor.Accounts.RestApi.AccountPaymentMethodController"/> and
/// <see cref="Andor.Accounts.RestApi.AccountSubCategoriesController"/> - the domain already
/// supported CreateCustomCategory/CreateCustomSubCategory/CreateCustomPaymentMethod, but there
/// was no HTTP surface for any of them until now.
/// </summary>
public sealed class CategoriesPaymentMethodsControllerTests : IClassFixture<AccountsApiFactory>
{
    private readonly AccountsApiFactory _factory;

    public CategoriesPaymentMethodsControllerTests(AccountsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAccount()
    {
        var (accountId, client) = await CreateAccountAsync();
        using var owner = client;

        var input = new CreateCategoryInput { Name = "Groceries", Description = "Food shopping", TypeId = MovementType.MoneySpending.Key };
        var response = await owner.PostAsJsonAsync($"v1/account/{accountId}/category", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        _ = body!.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateCategory_WithoutMembership_ReturnsBadRequest()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var owner = ownerClient;

        using var outsider = _factory.CreateAuthenticatedClient(new TestUser(Guid.NewGuid(), "Group"));
        var input = new CreateCategoryInput { Name = "Groceries", Description = "Food shopping", TypeId = MovementType.MoneySpending.Key };
        var response = await outsider.PostAsJsonAsync($"v1/account/{accountId}/category", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePaymentMethod_ReturnsCreatedAccount()
    {
        var (accountId, client) = await CreateAccountAsync();
        using var owner = client;

        var input = new CreatePaymentMethodInput { Name = "Debit Card", Description = "My debit card", TypeId = MovementType.MoneySpending.Key };
        var response = await owner.PostAsJsonAsync($"v1/account/{accountId}/payment-method", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        _ = body!.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateSubCategory_ForExistingCategory_ReturnsCreatedAccount()
    {
        var (accountId, client) = await CreateAccountAsync();
        using var owner = client;

        var categoryInput = new CreateCategoryInput { Name = "Food", Description = "Food category", TypeId = MovementType.MoneySpending.Key };
        await owner.PostAsJsonAsync($"v1/account/{accountId}/category", categoryInput, ComponentTestJson.Options);

        var categoryId = await GetSingleCategoryIdAsync(owner, accountId);

        var subCategoryInput = new CreateSubCategoryInput { Name = "Restaurants", Description = "Eating out", CategoryId = categoryId };
        var response = await owner.PostAsJsonAsync($"v1/account/{accountId}/sub-category", subCategoryInput, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        _ = body!.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateSubCategory_WithUnknownCategory_ReturnsBadRequest()
    {
        var (accountId, client) = await CreateAccountAsync();
        using var owner = client;

        var input = new CreateSubCategoryInput { Name = "Restaurants", Description = "Eating out", CategoryId = Guid.NewGuid() };
        var response = await owner.PostAsJsonAsync($"v1/account/{accountId}/sub-category", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ListCategoriesOutput derives from PaginatedListOutput<T>, whose constructor parameter names
    // (page, perPage, total, items) don't match its own init-only property names (CurrentPage,
    // PerPage, Total, Items) that System.Text.Json actually serializes - so deserializing it back
    // via the strongly-typed record throws a constructor-binding error. Read the id out of the raw
    // JSON instead of fighting that pre-existing DTO shape.
    private static async Task<Guid> GetSingleCategoryIdAsync(HttpClient client, Guid accountId)
    {
        var response = await client.GetAsync($"v1/account/{accountId}/category");
        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetString();
        return Guid.Parse(id!);
    }

    private async Task<(Guid accountId, HttpClient client)> CreateAccountAsync()
    {
        var currencyId = await GetSeededBrlCurrencyIdAsync();
        var client = _factory.CreateAuthenticatedClient();

        var input = new AccountInput("Category Test Account", currencyId.ToString());
        var response = await client.PostAsJsonAsync("v1/Account", input, ComponentTestJson.Options);
        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);

        return (Guid.Parse(body!.Data!.Id), client);
    }

    private async Task<Guid> GetSeededBrlCurrencyIdAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();
        var currency = await repository.GetByIsoAsync("BRL", CancellationToken.None);
        return currency!.Id;
    }
}

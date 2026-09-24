using System.Net;
using System.Net.Http.Json;
using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.Invites;
using Andor.Accounts.Contracts.Invites.Responses;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.ComponentTests.Common;
using Andor.Foundation.Contracts.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.ComponentTests;

/// <summary>
/// Component tests for <see cref="Andor.Accounts.RestApi.InvitesController"/>, proving the full
/// invite -> answer -> member-added cycle over real HTTP, with two distinct authenticated users
/// (owner and invitee).
/// </summary>
public sealed class InvitesControllerTests : IClassFixture<AccountsApiFactory>
{
    private readonly AccountsApiFactory _factory;

    public InvitesControllerTests(AccountsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateInvite_ByEmail_ReturnsInvite()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var client = ownerClient;

        var input = new InviteInput("newuser@example.com", null, PermissionType.Editor.Key);
        var response = await client.PostAsJsonAsync($"v1/account/{accountId}/invites", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);
        _ = body!.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateInvite_WithoutOwnerPermission_ReturnsBadRequest()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var owner = ownerClient;

        using var outsider = _factory.CreateAuthenticatedClient(new TestUser(Guid.NewGuid(), "Group"));
        var input = new InviteInput("newuser@example.com", null, PermissionType.Editor.Key);
        var response = await outsider.PostAsJsonAsync($"v1/account/{accountId}/invites", input, ComponentTestJson.Options);

        _ = response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AnswerInvite_Accept_AddsMemberToAccount()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var owner = ownerClient;
        var invitee = new TestUser(Guid.NewGuid(), "Group");

        var inviteInput = new InviteInput(null, invitee.Id, PermissionType.Editor.Key);
        var inviteResponse = await owner.PostAsJsonAsync($"v1/account/{accountId}/invites", inviteInput, ComponentTestJson.Options);
        _ = inviteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var inviteId = await GetSingleInviteIdAsync(owner, accountId);

        using var inviteeClient = _factory.CreateAuthenticatedClient(invitee);
        var answerResponse = await inviteeClient.PostAsJsonAsync(
            $"v1/account/{accountId}/invites/{inviteId}/answer", new AnswerInviteInput(true), ComponentTestJson.Options);

        _ = answerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var accountResponse = await owner.GetAsync($"v1/Account/{accountId}");
        var accountBody = await accountResponse.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);

        _ = accountBody!.Data!.Participants.Should().Contain(p => p.Id == invitee.Id.ToString());
    }

    [Fact]
    public async Task AnswerInvite_Reject_DoesNotAddMember()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var owner = ownerClient;
        var invitee = new TestUser(Guid.NewGuid(), "Group");

        var inviteInput = new InviteInput(null, invitee.Id, PermissionType.Editor.Key);
        await owner.PostAsJsonAsync($"v1/account/{accountId}/invites", inviteInput, ComponentTestJson.Options);

        var inviteId = await GetSingleInviteIdAsync(owner, accountId);

        using var inviteeClient = _factory.CreateAuthenticatedClient(invitee);
        var answerResponse = await inviteeClient.PostAsJsonAsync(
            $"v1/account/{accountId}/invites/{inviteId}/answer", new AnswerInviteInput(false), ComponentTestJson.Options);

        _ = answerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var accountResponse = await owner.GetAsync($"v1/Account/{accountId}");
        var accountBody = await accountResponse.Content.ReadFromJsonAsync<DefaultResponse<AccountOutput>>(ComponentTestJson.Options);

        _ = accountBody!.Data!.Participants.Should().NotContain(p => p.Id == invitee.Id.ToString());
    }

    [Fact]
    public async Task ListInvites_ReturnsPendingInvites()
    {
        var (accountId, ownerClient) = await CreateAccountAsync();
        using var client = ownerClient;

        var input = new InviteInput("newuser@example.com", null, PermissionType.Viewer.Key);
        await client.PostAsJsonAsync($"v1/account/{accountId}/invites", input, ComponentTestJson.Options);

        var response = await client.GetAsync($"v1/account/{accountId}/invites");
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DefaultResponse<ListInviteOutput>>(ComponentTestJson.Options);
        _ = body!.Data!.Items.Should().ContainSingle(i => i.Email == "newuser@example.com" && i.IsActive);
    }

    private static async Task<string> GetSingleInviteIdAsync(HttpClient ownerClient, Guid accountId)
    {
        var listResponse = await ownerClient.GetAsync($"v1/account/{accountId}/invites");
        var listBody = await listResponse.Content.ReadFromJsonAsync<DefaultResponse<ListInviteOutput>>(ComponentTestJson.Options);
        return listBody!.Data!.Items.Single().Id;
    }

    private async Task<(Guid accountId, HttpClient client)> CreateAccountAsync()
    {
        var currencyId = await GetSeededBrlCurrencyIdAsync();
        var client = _factory.CreateAuthenticatedClient();

        var input = new AccountInput("Invite Test Account", currencyId.ToString());
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

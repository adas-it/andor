namespace Family.Budget.Infrastructure.Services.Keycloak;

using Family.Budget.Application.Common;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.Entities.Users.ValueObject;
using Family.Budget.Infrastructure.Gateway.Keycloak.Models;
using Mapster;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using Attributes = Gateway.Keycloak.Models.Attributes;

public class KeycloackService : IKeycloackService
{
    private readonly IKeycloackClient _keycloackClient;
    private readonly IOptions<ApplicationSettings> _configuration;

    public KeycloackService(IKeycloackClient keycloackClient, IOptions<ApplicationSettings> configuration)
    {
        _keycloackClient = keycloackClient;
        _configuration = configuration;
    }

    public async Task<User> CreateUser(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password,
        LocationInfos DefaultLanguage,
        string? Avatar,
        CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloack!.Realm;

        var requestDto = new CreateUser(true,
            Username ?? Email,
            Email,
            FirstName,
            LastName,
            new List<Credentials>() { new Credentials("password", Password, false) },
            new Attributes(new string[] { JsonConvert.SerializeObject(DefaultLanguage) },
                new string[] { Avatar ?? string.Empty },
                new string[] { DateTime.UtcNow.ToString() },
                new string[] { DateTime.UtcNow.ToString() }));

        var response = await _keycloackClient.CreateUser(requestDto, realm!, cancellation);

        var location = response.Headers.GetValues("Location").FirstOrDefault();

        var userId = location!.Split("/").Last();

        var item = User.New(Guid.Parse(userId), Username!,
        true, true, FirstName, LastName, Email, Avatar ?? string.Empty, DateTime.UtcNow,
        true, DateTime.UtcNow, true, DateTime.UtcNow, DefaultLanguage);

        return item;
    }

    public async Task<List<User>?> GetUserByEmail(string email, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloack!.Realm;

        var response = await _keycloackClient.Get(realm!, email, null!, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<List<User>?> GetUserByUserName(string username, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloack!.Realm;

        var response = await _keycloackClient.Get(realm!, null!, username, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloack!.Realm;

        var response = await _keycloackClient.Get(realm!, userId, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).FirstOrDefault();

        return ret!;
    }
}

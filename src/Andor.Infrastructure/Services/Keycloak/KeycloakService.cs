using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Domain.Entities.Currencies;
using Andor.Domain.Entities.Languages;
using Andor.Domain.Entities.Users;
using Andor.Domain.Entities.Users.ValueObjects;
using Andor.Infrastructure.Services.Keycloak.Models;
using Mapster;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Andor.Infrastructure.Services.Keycloak;

public class KeycloakService : IKeycloakService
{
    private readonly IKeycloakClient _keycloackClient;
    private readonly IOptions<ApplicationSettings> _configuration;

    public KeycloakService(IKeycloakClient keycloakClient, IOptions<ApplicationSettings> configuration)
    {
        _keycloackClient = keycloakClient;
        _configuration = configuration;
    }

    public async Task<User> CreateUser(string Username,
        MailAddress Email,
        string FirstName,
        string LastName,
        string Password,
        Currency DefaultCurrency,
        Language DefaultLanguage,
        string? Avatar,
        CancellationToken cancellationToken)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var requestDto = new CreateUser(true,
        Username ?? Email.Address,
        Email.Address,
        FirstName,
        LastName,
        new List<Credentials>() { new Credentials("password", Password, false) },
        new Attributes(
            new string[] { JsonConvert.SerializeObject(DefaultCurrency) },
            new string[] { JsonConvert.SerializeObject(DefaultLanguage) },
            new string[] { Avatar ?? string.Empty },
            new string[] { DateTime.UtcNow.ToString() },
            new string[] { DateTime.UtcNow.ToString() }));

        var response = await _keycloackClient.CreateUser(requestDto, realm!, cancellationToken);

        var location = response.Headers.GetValues("Location").FirstOrDefault();

        var userId = location!.Split("/").Last();

        var item = User.New(UserId.Load(userId), Username!,
        true, true, FirstName, LastName, Email, Avatar ?? string.Empty, DateTime.UtcNow,
        true, DateTime.UtcNow, true, DateTime.UtcNow, DefaultCurrency, DefaultLanguage);

        return item;
    }

    public async Task<List<User>?> GetUserByEmail(MailAddress email, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloackClient.Get(realm!, email.Address, null!, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<List<User>?> GetUserByUserName(string username, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloackClient.Get(realm!, null!, username, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloackClient.Get(realm!, userId, cancellation);

        if (response.Any() is false)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).FirstOrDefault();

        return ret!;
    }
}

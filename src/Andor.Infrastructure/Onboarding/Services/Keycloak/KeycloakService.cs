using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Domain.Onboarding.Users;
using Andor.Domain.Onboarding.Users.ValueObjects;
using Andor.Infrastructure.Onboarding.Services.Keycloak.Models;
using Mapster;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace Andor.Infrastructure.Onboarding.Services.Keycloak;

public class KeycloakService
    (IKeycloakClient _keycloakClient, IOptions<ApplicationSettings> _configuration)
    : IKeycloakService
{
    public async Task<User> CreateUser(string Username,
        MailAddress Email,
        string FirstName,
        string LastName,
        string Password,
        CancellationToken cancellationToken)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var requestDto = new CreateUser(
            FirstName: FirstName,
            LastName: LastName,
            Email: Email.Address,
            Enabled: true,
            EmailVerified: true,
            Username: Username,
            [new Credentials("password", Password, false)],
            new Attributes(
            [string.Empty],
            [DateTime.UtcNow.ToString()],
            [DateTime.UtcNow.ToString()]),
            ["basic"]
            );

        var response = await _keycloakClient.CreateUser(requestDto, realm!, cancellationToken);

        var location = response.Headers.GetValues("Location").FirstOrDefault();

        var userId = location!.Split("/").Last();

        var item = User.New(UserId.Load(userId), Username!,
        true, true, FirstName, LastName, Email, string.Empty, DateTime.UtcNow,
        true, DateTime.UtcNow, true, DateTime.UtcNow, null!, null!);

        return item;
    }

    public async Task<List<User>?> GetUserByEmail(MailAddress email, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloakClient.Get(realm!, email.Address, null!, cancellation);

        if (response.Count != 0)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<List<User>?> GetUserByUserName(string username, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloakClient.Get(realm!, null!, username, cancellation);

        if (response.Count != 0)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).ToList();

        return ret!;
    }

    public async Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloakClient.Get(realm!, userId, cancellation);

        if (response.Count != 0)
            return null;

        var ret = response.Select(x => x.Adapt<User>()).FirstOrDefault();

        return ret!;
    }
}

using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Domain.Administrations.Languages;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
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
    public async Task<User> CreateUser(Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string Password,
    string Email,
    Guid Locale,
    Language Language,
    Currency Currency,
    bool AcceptedTermsCondition,
    bool AcceptedPrivateData,
        CancellationToken cancellationToken)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var requestDto = new CreateUser(
            FirstName: FirstName,
            LastName: LastName,
            Email: Email,
            Enabled: true,
            EmailVerified: true,
            Username: UserName,
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

        var item = User.New(UserId.Load(userId), UserName,
        true, true, FirstName, LastName, new MailAddress(Email), string.Empty, DateTime.UtcNow,
        true, DateTime.UtcNow, true, DateTime.UtcNow, Currency, Language);

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

    public async Task<List<User>?> GetUserByUserName(string userName, CancellationToken cancellation)
    {
        var realm = _configuration.Value.Keycloak!.Realm;

        var response = await _keycloakClient.Get(realm!, null!, userName, cancellation);

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

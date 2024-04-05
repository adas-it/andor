using Andor.Application.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Andor.Infrastructure.Onboarding.Services.Keycloak.Auth;

public class KeycloakAuthenticationHandler : DelegatingHandler
{
    private readonly IKeycloakAuthClient _keycloakClient;
    private readonly IOptions<ApplicationSettings> _configuration;

    public KeycloakAuthenticationHandler(IKeycloakAuthClient keycloakClient, IOptions<ApplicationSettings> configuration)
    {
        _keycloakClient = keycloakClient;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (tokenType, token) = await KeycloackAdminLogin(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);

        return await base.SendAsync(request, cancellationToken);
    }

    public async Task<(string, string)> KeycloackAdminLogin(CancellationToken cancellation)
    {
        var clientId = _configuration?.Value?.Keycloak?.ClientId;
        var clientSecret = _configuration?.Value?.Keycloak?.ClientSecret;
        var grantType = _configuration?.Value?.Keycloak?.GrantType;
        var realm = _configuration?.Value?.Keycloak?.Realm;

        var contentKey = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!),
            new KeyValuePair<string, string>("grant_type", grantType!)
        };

        var content = new FormUrlEncodedContent(contentKey);

        var response = await _keycloakClient.GetToken(content, realm!, cancellation);

        return ("Bearer", response.access_token);
    }
}

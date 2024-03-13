namespace Family.Budget.Infrastructure.Services.Keycloak.Auth;
using Family.Budget.Application.Common;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class KeycloackAuthenticationHandler : DelegatingHandler
{
    private readonly IKeycloakAuthClient _keycloackClient;
    private readonly IOptions<ApplicationSettings> _configuration;

    public KeycloackAuthenticationHandler(IKeycloakAuthClient keycloackClient, IOptions<ApplicationSettings> configuration)
    {
        _keycloackClient = keycloackClient;
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
        var clientId = _configuration?.Value?.Keycloack?.ClientId;
        var clientSecret = _configuration?.Value?.Keycloack?.ClientSecret;
        var grantType = _configuration?.Value?.Keycloack?.GrantType;
        var realm = _configuration?.Value?.Keycloack?.Realm;

        var contentKey = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!),
            new KeyValuePair<string, string>("grant_type", grantType!)
        };

        var content = new FormUrlEncodedContent(contentKey);

        var response = await _keycloackClient.GetToken(content, realm!, cancellation);

        return ("Bearer", response.access_token);
    }
}

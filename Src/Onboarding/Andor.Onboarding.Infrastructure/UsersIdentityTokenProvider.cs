using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Andor.Onboarding.Infrastructure;

/// <summary>
/// Client-credentials token for the "onboarding-service" OpenIddict client registered in
/// Andor.Users.WebApi — obtained once and cached until shortly before it expires, since the
/// caller (a hard dependency of Verify) can't afford a token round-trip on every request.
/// </summary>
public interface IUsersIdentityTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}

public sealed class UsersIdentityClientOptions
{
    public const string SectionName = "UsersIdentityClient";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "users.write";
}

public sealed class UsersIdentityTokenProvider(HttpClient httpClient, IOptions<UsersIdentityClientOptions> options)
    : IUsersIdentityTokenProvider
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
        {
            return _accessToken;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
            {
                return _accessToken;
            }

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = options.Value.ClientId,
                ["client_secret"] = options.Value.ClientSecret,
                ["scope"] = options.Value.Scope,
            };

            using var response = await httpClient.PostAsync(
                "connect/token", new FormUrlEncodedContent(form), cancellationToken);

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                ?? throw new InvalidOperationException("Empty token response from the identity server.");

            _accessToken = payload.AccessToken;
            // Refresh a bit before actual expiry so an in-flight request never races a token that's
            // about to lapse mid-call.
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(payload.ExpiresIn - 30, 30));

            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}

// Ignore Spelling: Keycloak Cors Mq Jwt

namespace Andor.Application.Common;

public record ApplicationSettings
{
    public IdentityProvider? IdentityProvider { get; init; }
    public Cors? Cors { get; init; }
    public OpenTelemetryConfig? OpenTelemetryConfig { get; init; }
    public Keycloak? Keycloak { get; init; }
    public SmtpConfig? SmtpConfig { get; init; }
}

public record Cors(List<string>? AllowedOrigins);

public record IdentityProvider
{
    public List<string>? Scopes { get; init; }
    public string? SecretKey { get; init; }
    public string? Authority { get; init; }
    public string? SwaggerClientId { get; init; }
    public string? PublicKeyJwt { get; set; }
}

public record OpenTelemetryConfig(string? StatusGaugeName, string? DurationGaugeName, string? Endpoint);

public record Keycloak(string? Url, string? Realm, string? ClientId, string? ClientSecret, string? GrantType);

public record PollyConfigs(string Repetitions, string TimeCircuitBreak, string TimeOut)
{
    public const string PollyConfig = "Polly";
}

public record SmtpConfig
{
    public string? Username { get; set; }
    public string? Smtp { get; set; }
    public int? Port { get; set; }
    public string? DisplayName { get; set; }
    public string? Password { get; set; }
}

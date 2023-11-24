namespace Family.Budget.Application.Common
{
    public class ApplicationSettings
    {
        public IdentityProvider? IdentityProvider { get; init; }
        public HangfireDashboard? HangfireDashboard { get; init; }
        public Rabbitmq? Rabbitmq { get; init; }
        public Cors? Cors { get; init; }
        public Keycloack? Keycloack { get; init; }
    }

    public class Cors
    {
        public List<string>? AllowedOrigins { get; set; }
    }

    public class HangfireDashboard
    {
        public string? User { get; init; }
        public string? Password { get; init; }
    }

    public class IdentityProvider
    {
        public List<string>? Scopes { get; init; }
        public string? SecretKey { get; init; }
        public string? Authority { get; init; }
        public string? SwaggerClientId { get; init; }
        public string? PublicKeyJwt { get; set; }
    }

    public class Rabbitmq
    {
        public string? HostName { get; init; }
        public string? HostPort { get; init; }
        public string? VirtualHost { get; init; }
        public string? Username { get; init; }
        public string? Password { get; init; }
    }
    public class Keycloack
    {
        public string? Url { get; init; }
        public string? Realm { get; init; }
        public string? ClientId { get; init; }
        public string? ClientSecret { get; init; }
        public string? GrantType { get; init; }
        public string? Username { get; init; }
        public string? Password { get; init; }
    }
}

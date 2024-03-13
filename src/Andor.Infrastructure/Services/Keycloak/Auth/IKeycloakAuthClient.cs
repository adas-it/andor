using Andor.Infrastructure.Services.Keycloak.Models;
using Refit;

namespace Andor.Infrastructure.Services.Keycloak.Auth;

public interface IKeycloakAuthClient
{
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<LoginResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] FormUrlEncodedContent content, string realm, CancellationToken cancellationToken);
}

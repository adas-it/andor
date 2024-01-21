namespace Family.Budget.Infrastructure;
using Refit;
using System.Threading.Tasks;
using Family.Budget.Infrastructure.Services.Keycloak.Models;

public interface IKeycloakAuthClient
{
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<LoginResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] FormUrlEncodedContent content, string realm, CancellationToken cancellationToken);
}

namespace Family.Budget.Infrastructure.Services.Keycloak;

using Refit;
using System.Threading.Tasks;
using Family.Budget.Infrastructure.Gateway.Keycloak.Models;
using Family.Budget.Infrastructure.Services.Keycloak.Models;
using Family.Budget.Infrastructure.Services.Keycloak.Models.Response;

public interface IKeycloackClient
{
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<LoginResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] FormUrlEncodedContent content, string realm, CancellationToken cancellationToken);

    [Post("/admin/realms/{realm}/users")]
    Task<HttpResponseMessage> CreateUser([Body(BodySerializationMethod.Serialized)] CreateUser createUser, string realm, CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/users")]
    Task<List<UserResponse>> Get(string realm, string? email, string? username, CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/users/{userId}")]
    Task<List<UserResponse>> Get(string realm, Guid userId, CancellationToken cancellationToken);
}

namespace Andor.Infrastructure.Onboarding.Services.Keycloak;

using Andor.Infrastructure.Onboarding.Services.Keycloak.Models;
using Andor.Infrastructure.Onboarding.Services.Keycloak.Models.Response;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IKeycloakClient
{
    [Post("/admin/realms/{realm}/users")]
    Task<HttpResponseMessage> CreateUser([Body] CreateUser createUser, string realm, CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/users")]
    Task<List<UserResponse>> Get(string realm, string? email, string? username, CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/users/{userId}")]
    Task<List<UserResponse>> Get(string realm, Guid userId, CancellationToken cancellationToken);
}

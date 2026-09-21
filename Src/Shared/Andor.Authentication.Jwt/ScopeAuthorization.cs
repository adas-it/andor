using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace Andor.Authentication.Jwt;

/// <summary>
/// Satisfied when the caller's token carries the given OAuth2 scope. Register a named policy
/// against it (e.g. <c>options.AddPolicy("users.write", p => p.Requirements.Add(new ScopeRequirement("users.write")))</c>)
/// in the resource server that needs it — <see cref="JwtAuthenticationMiddleware.ConfigureJwt"/>
/// only wires the handler; it has no opinion on which scopes any particular endpoint requires.
/// </summary>
public sealed class ScopeRequirement(string scope) : IAuthorizationRequirement
{
    public string Scope { get; } = scope;
}

public sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        if (context.User.HasScope(requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

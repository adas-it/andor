using Andor.Authorizations.Domain;

namespace Andor.Authorizations.Application;

internal class AuthorizationRepository : IAuthorizationRepository
{
    public List<Permission> Permissions = new();

    public Task<List<Permission>> GetPermissions()
        => Task.FromResult(Permissions);

    public Task SavePermission(Permission permissions)
    {
        Permissions.Add(permissions);

        return Task.CompletedTask;
    }
}

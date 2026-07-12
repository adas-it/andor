namespace Andor.Authorizations.Domain;

public interface IAuthorizationRepository
{
    Task SavePermission(Permission permissions);
    Task<List<Permission>> GetPermissions();
}

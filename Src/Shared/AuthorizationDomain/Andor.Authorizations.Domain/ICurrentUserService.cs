namespace Andor.Authorizations.Domain;

public interface ICurrentUserService
{
    ApplicationUser GetCurrentUser();
}

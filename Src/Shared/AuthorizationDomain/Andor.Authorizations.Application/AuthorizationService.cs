using Andor.Authorizations.Domain;

namespace Andor.Authorizations.Application;

public class AuthorizationService : IAuthorizationService
{
    public async Task<LicenseType> GetLicenseTypeAsync()
    {
        return LicenseType.Enterprise;
    }
}

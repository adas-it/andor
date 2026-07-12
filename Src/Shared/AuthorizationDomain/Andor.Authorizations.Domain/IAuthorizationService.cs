namespace Andor.Authorizations.Domain;

public interface IAuthorizationService
{
    Task<LicenseType> GetLicenseTypeAsync();
}

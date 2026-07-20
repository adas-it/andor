namespace Andor.Authorizations.Domain;

public class AuthorizationDomain
{
    private readonly IAuthorizationRepository authorizationRepository;
    private readonly IAuthorizationService authorizationService;
    private readonly ICurrentUserService currentUserService;
    private readonly ICollection<Permission> _permissions;

    public AuthorizationDomain(IAuthorizationRepository authorizationRepository,
        IAuthorizationService authorizationService,
        ICurrentUserService currentUserService)
    {
        _permissions = authorizationRepository.GetPermissions().GetAwaiter().GetResult();

        this.authorizationRepository = authorizationRepository;
        this.authorizationService = authorizationService;
        this.currentUserService = currentUserService;
    }

    public Task<bool> IsAuthorizedAsync(string permissionName, Dictionary<string, string>? parameters)
    {
        var currentUser = currentUserService.GetCurrentUser();

        var permission = _permissions.FirstOrDefault(x =>
            x.Name == permissionName && x.GroupName == currentUser.GroupName);

        return Task.FromResult(permission?.Allowed ?? false);
    }

    public async Task<bool> AddPermissionAsync(string permissionName, Dictionary<string, string>? parameters)
    {
        var permission = new Permission { Name = permissionName, Allowed = true };

        _permissions.Add(permission);

        await authorizationRepository.SavePermission(permission);

        return true;
    }

}

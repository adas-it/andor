using Andor.Authorizations.Domain;

namespace Andor.Authorizations.Application;

public class UserContextAccessor : IUserContextAccessor
{
    private static readonly AsyncLocal<ApplicationUser?> _currentUser = new();

    public ApplicationUser? CurrentUser
    {
        get => _currentUser.Value;
        set => _currentUser.Value = value;
    }
}
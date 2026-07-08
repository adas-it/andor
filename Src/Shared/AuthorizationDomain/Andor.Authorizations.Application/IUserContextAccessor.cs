using Andor.Authorizations.Domain;

namespace Andor.Authorizations.Application;

public interface IUserContextAccessor
{
    ApplicationUser? CurrentUser { get; set; }
}

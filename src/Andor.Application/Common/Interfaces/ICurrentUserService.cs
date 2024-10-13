using Andor.Application.Common.Models.Authorizations;

namespace Andor.Application.Common.Interfaces;

public interface ICurrentUserService
{
    ApplicationUser User { get; }
}

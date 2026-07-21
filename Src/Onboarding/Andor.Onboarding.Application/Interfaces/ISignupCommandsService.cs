using Andor.Authorizations.Domain;
using Andor.Foundation.Contracts.Results;

namespace Andor.Onboarding.Application.Interfaces;

public interface ISignupCommandsService
{
    Task<ApplicationResult<object?>> StartSignupAsync(string name, string email,
        ApplicationUser currentUser, CancellationToken cancellationToken);

    Task<ApplicationResult<object?>> VerifySignupAsync(string email, string code, string password,
        ApplicationUser currentUser, CancellationToken cancellationToken);
}

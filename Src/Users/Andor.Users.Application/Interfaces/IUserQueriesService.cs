using Andor.Users.Contracts.Responses;
using Andor.Foundation.Contracts.Results;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application.Interfaces;

public interface IUserQueriesService
{
    Task<ApplicationResult<UserPreferencesOutput?>> GetByIdAsync(UserId id, CancellationToken cancellationToken);
}

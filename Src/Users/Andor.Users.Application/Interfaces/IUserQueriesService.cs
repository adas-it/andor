using Andor.Foundation.Contracts.Results;
using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application.Interfaces;

public interface IUserQueriesService
{
    Task<ApplicationResult<UserPreferencesOutput?>> GetByIdAsync(UserId id, CancellationToken cancellationToken);

    Task<ApplicationResult<UserPublicData?>> GetPublicDataByIdAsync(UserId id,
        CancellationToken cancellationToken);
}

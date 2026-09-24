using Andor.Foundation.Contracts.Results;
using Andor.Users.Application.Interfaces;
using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application;

public class UserQueriesService(IQueriesUserRepository repository) : IUserQueriesService
{
    public async Task<ApplicationResult<UserPreferencesOutput?>> GetByIdAsync(UserId id,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);

        return ApplicationResult<UserPreferencesOutput?>.Success(Data: user.ToUserPreferencesOutput());
    }

    public async Task<ApplicationResult<UserPublicData?>> GetPublicDataByIdAsync(UserId id,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);

        return ApplicationResult<UserPublicData?>.Success(Data: user.ToUserPublicData());
    }
}

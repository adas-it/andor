using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users;

namespace Andor.Users.Application;

internal static class UserMapperExtensions
{
    public static UserPreferencesOutput? ToUserPreferencesOutput(this User? user)
    {
        if (user is null)
        {
            return null;
        }

        return new UserPreferencesOutput(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.PreferredCurrencyId,
            user.PreferredLanguageId,
            user.Avatar,
            user.AvatarThumbnail);
    }

    public static UserPublicData? ToUserPublicData(this User? user)
    {
        if (user is null)
        {
            return null;
        }

        return new UserPublicData(
            user.Id,
            user.FirstName,
            user.Avatar,
            user.AvatarThumbnail);
    }
}

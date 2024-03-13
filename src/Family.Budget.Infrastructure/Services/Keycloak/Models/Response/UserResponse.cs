using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.Entities.Users.ValueObject;
using Newtonsoft.Json;

namespace Family.Budget.Infrastructure.Services.Keycloak.Models.Response;
public record UserResponse(
    Guid Id,
    long createdTimestamp,
    string Username,
    bool Enabled,
    string FirstName,
    string LastName,
    string Email,
    bool EmailVerified,
    Attributes Attributes);

public record Attributes(string[] Locale,
    string[] Avatar,
    string[] AcceptedPrivateData,
    string[] AcceptedPrivateDataDate,
    string[] AcceptedTermsCondition,
    string[] AcceptedTermsConditionDate
    );

public static class UserResponseAdapter
{
    public static User? Projection(this UserResponse? item)
        => item == null ? null :
        User.New(item.Id,
            item.Username,
            item.Enabled,
            item.EmailVerified,
            item.FirstName,
            item.LastName,
            item.Email,
            item.Attributes.Avatar.FirstOrDefault()!,
            UnixTimeStampToDateTime(item.createdTimestamp),
            bool.Parse(item.Attributes.AcceptedTermsCondition.FirstOrDefault()!),
            DateTime.Parse(item.Attributes.AcceptedTermsConditionDate.FirstOrDefault()!),
            bool.Parse(item.Attributes.AcceptedPrivateData.FirstOrDefault()!),
            DateTime.Parse(item.Attributes.AcceptedPrivateDataDate.FirstOrDefault()!),
                JsonConvert.DeserializeObject<LocationInfos>(item.Attributes.Locale[0])
            );

    public static DateTimeOffset UnixTimeStampToDateTime(long unixTimeStamp)
    {
        DateTimeOffset rslt = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).DateTime;
        return rslt;
    }
}

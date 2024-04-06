using Andor.Domain.Entities.Currencies;
using Andor.Domain.Entities.Languages;
using Andor.Domain.Entities.Users;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Andor.Infrastructure.Onboarding.Services.Keycloak.Models.Response;
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

public record Attributes(string[] Currency,
    string[] Language,
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
            new MailAddress(item.Email),
            item.Attributes.Avatar.FirstOrDefault()!,
            UnixTimeStampToDateTime(item.createdTimestamp),
            bool.Parse(item.Attributes.AcceptedTermsCondition.FirstOrDefault()!),
            DateTime.Parse(item.Attributes.AcceptedTermsConditionDate.FirstOrDefault()!),
            bool.Parse(item.Attributes.AcceptedPrivateData.FirstOrDefault()!),
            DateTime.Parse(item.Attributes.AcceptedPrivateDataDate.FirstOrDefault()!),
                JsonConvert.DeserializeObject<Currency>(item.Attributes.Currency[0]),
                JsonConvert.DeserializeObject<Language>(item.Attributes.Language[0])
            );

    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        DateTime rslt = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).DateTime;
        return rslt;
    }
}
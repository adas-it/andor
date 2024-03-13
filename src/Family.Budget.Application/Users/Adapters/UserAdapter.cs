namespace Family.Budget.Application.Users.Adapters;
using Family.Budget.Application.Dto.Users.Responses;
using Family.Budget.Domain.Entities.Users;

public static class UserAdapter
{
    public static UserOutput MapDtoFromDomain(this User item)
        => new(item.Id, item.UserName, item.Enabled, item.EmailVerified,
            item.FirstName, item.LastName, item.Email,
            item.Avatar, item.LocationInfos.PreferedLanguage, item.AcceptedTermsCondition, item.AcceptedPrivateData);
}

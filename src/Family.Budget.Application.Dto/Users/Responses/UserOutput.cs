namespace Family.Budget.Application.Dto.Users.Responses;
public record UserOutput
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public bool Enabled { get; set; }
    public bool EmailVerified { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public string Locale { get; set; }
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }

    public UserOutput(Guid id, string userName, bool enabled, bool emailVerified, string firstName, string lastName, string email, string avatar, string locale, bool acceptedTermsCondition, bool acceptedPrivateData)
    {
        Id = id;
        UserName = userName;
        Enabled = enabled;
        EmailVerified = emailVerified;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Avatar = avatar;
        Locale = locale;
        AcceptedTermsCondition = acceptedTermsCondition;
        AcceptedPrivateData = acceptedPrivateData;
    }

}

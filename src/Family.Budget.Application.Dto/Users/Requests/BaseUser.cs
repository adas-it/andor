namespace Family.Budget.Application.Dto.Users.Requests;

public abstract record BaseUser
{
    public string? UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Avatar { get; set; }
    public string? Locale { get; set; }
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }

    public BaseUser()
    {
    }
    protected BaseUser(string userName, string firstName, string lastName, string email, string avatar, string locale, bool acceptedTermsCondition, bool acceptedPrivateData)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Avatar = avatar;
        Locale = locale;
        AcceptedTermsCondition = acceptedTermsCondition;
        AcceptedPrivateData = acceptedPrivateData;
    }
}

namespace Family.Budget.Application.Dto.Users.Requests;

public record RegisterUserInput : BaseUser
{
    public RegisterUserInput(string? userName, 
        string password, 
        string code, 
        string firstName, 
        string lastName, 
        string email, 
        string? avatar, 
        string? locale, 
        bool acceptedTermsCondition, 
        bool acceptedPrivateData) : base(userName,firstName, lastName, email, avatar, locale, acceptedTermsCondition, acceptedPrivateData)
    {
        Password = password;
        Code = code;
    }

    public string Password { get; set; }
    public string Code { get; set; }
}

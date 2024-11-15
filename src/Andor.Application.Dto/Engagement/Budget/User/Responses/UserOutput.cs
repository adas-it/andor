namespace Andor.Application.Dto.Engagement.Budget.User.Responses;

public record UserOutput
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Birthday { get; set; }
    public string Avatar { get; set; }
    public string AvatarThumb { get; set; }
    public Guid PreferredCurrencyId { get; set; }
    public Guid PreferredLanguageId { get; set; }
}

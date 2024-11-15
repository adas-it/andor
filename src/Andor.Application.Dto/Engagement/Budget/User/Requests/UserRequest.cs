namespace Andor.Application.Dto.Engagement.Budget.User.Requests;

public record UserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid PreferredCurrencyId { get; set; }
    public Guid PreferredLanguageId { get; set; }
}

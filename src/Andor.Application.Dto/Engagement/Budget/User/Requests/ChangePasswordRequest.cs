namespace Andor.Application.Dto.Engagement.Budget.User.Requests;

public record ChangePasswordRequest
{
    public string Current { get; set; }
    public string New { get; set; }
}

namespace Andor.Application.Dto.Engagement.Budget.Invites.Requests;
public record InviteInput
{
    public string Email { get; set; }
}

public record AnswerInput
{
    public bool IsAccepted { get; set; }
}
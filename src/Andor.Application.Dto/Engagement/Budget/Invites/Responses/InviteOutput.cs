namespace Andor.Application.Dto.Engagement.Budget.Invites.Responses;

public record InviteOutput
{
    public string Email { get; set; }
    public InviteOutputStatusOutput Status { get; set; }
    public Guid? AccountId { get; set; }
}

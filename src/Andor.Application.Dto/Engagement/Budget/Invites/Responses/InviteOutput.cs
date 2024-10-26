namespace Andor.Application.Dto.Engagement.Budget.Invites.Responses;

public record InviteOutput
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public InviteOutputStatusOutput Status { get; set; }
    public Guid? AccountId { get; set; }
}

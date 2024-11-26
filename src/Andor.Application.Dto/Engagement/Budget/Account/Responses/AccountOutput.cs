namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public record AccountOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool Deleted { get; set; }
    public DateTime? FirstMovement { get; set; }
    public DateTime? LastMovement { get; set; }
    public List<ParticipantOutput> Participants { get; set; }
}

public record ParticipantOutput
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
    public string AvatarThumbnail { get; set; }
    public ParticipantStatusOutput Status { get; set; }
}

public record ParticipantStatusOutput(int Key, string Name);
namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public record AccountOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool Deleted { get; set; }
    public DateTime? FirstMovement { get; set; }
    public DateTime? LastMovement { get; set; }
}

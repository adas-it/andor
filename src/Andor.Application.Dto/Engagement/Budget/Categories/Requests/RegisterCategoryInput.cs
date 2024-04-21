namespace Andor.Application.Dto.Engagement.Budget.Categories.Requests;

public record CategoryInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public int MovementTypeId { get; set; }
}

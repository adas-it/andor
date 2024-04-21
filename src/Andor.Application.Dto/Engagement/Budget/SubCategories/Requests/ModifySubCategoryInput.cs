namespace Andor.Application.Dto.Engagement.Budget.SubCategories.Requests;

public record ModifySubCategoryInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public Guid CategoryId { get; set; }
}

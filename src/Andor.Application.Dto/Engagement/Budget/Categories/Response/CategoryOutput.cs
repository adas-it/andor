namespace Andor.Application.Dto.Engagement.Budget.Categories.Response;

public record CategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DeactivationDate { get; set; }
    public CategoryTypeOutput Type { get; set; }
    public string Avatar { get; set; }
}

public record CategoryTypeOutput(int Key, string Name);
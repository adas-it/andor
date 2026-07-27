namespace Andor.Accounts.Contracts.Categories.Response;

public record CategoryOutput
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CategoryTypeOutput Type { get; set; }
    public int? Order { get; set; }
}

public record CategoryTypeOutput(int Key, string Name);

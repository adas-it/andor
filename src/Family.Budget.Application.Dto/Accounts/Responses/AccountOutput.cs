namespace Family.Budget.Application.Dto.Accounts.Responses;

public sealed record AccountOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset FirstMovement { get; set; }
    public DateTimeOffset LastMovement { get; set; }
}

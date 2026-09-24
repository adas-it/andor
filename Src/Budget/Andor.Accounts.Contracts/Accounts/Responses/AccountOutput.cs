namespace Andor.Accounts.Contracts.Accounts.Responses;

public record AccountOutput
{
    public string Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public CurrencyOutput Currency { get; set; }
    public bool Deleted { get; set; }
    public DateTime? FirstMovement { get; set; }
    public DateTime? LastMovement { get; set; }
    public List<ParticipantOutput> Participants { get; set; }
}

public record CurrencyOutput(
    string Id,
    string Name,
    string Iso
);

public record ParticipantOutput
{
    public string Id { get; set; }
    public PermissionTypeOutput PermissionType { get; set; }
}

public record PermissionTypeOutput(int Key, string Name);

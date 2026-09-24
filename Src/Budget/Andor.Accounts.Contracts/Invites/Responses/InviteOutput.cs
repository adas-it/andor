namespace Andor.Accounts.Contracts.Invites.Responses;

public record InviteOutput
{
    public string Id { get; set; }
    public string AccountId { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public int PermissionKey { get; set; }
    public string PermissionName { get; set; }
    public bool IsActive { get; set; }
    public bool IsAccepted { get; set; }
}

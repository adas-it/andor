namespace Andor.Authorizations.Domain;

public class ApplicationUser(Guid userId, string groupName, bool isAuthenticated, string tenant)
{
    public Guid UserId { get; } = userId;
    public string GroupName { get; } = groupName;
    public bool IsAuthenticated { get; } = isAuthenticated;
    public string Tenant { get; } = tenant;
}

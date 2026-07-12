namespace Andor.Authorizations.Domain;

public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string GroupName { get; set; }
    public LicenseType LicenseType { get; set; }
    public bool Allowed { get; set; }
    public bool Default { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}

using Andor.Foundation.Domain;

namespace Andor.Accounts.Domain.PermissionTypes;

public record PermissionType : Enumeration<int>
{
    private PermissionType(int key, string name) : base(key, name)
    {
    }

    public static readonly PermissionType Undefined = new(0, "undefined");
    public static readonly PermissionType Viewer = new(1, "viewer");
    public static readonly PermissionType Editor = new(2, "editor");
    public static readonly PermissionType Owner = new(2, "owner");
}
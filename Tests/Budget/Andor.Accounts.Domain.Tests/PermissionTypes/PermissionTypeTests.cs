using Andor.Accounts.Domain.PermissionTypes;

namespace Andor.Accounts.Domain.Tests.PermissionTypes;

public class PermissionTypeTests
{
    [Fact]
    public void Owner_And_Editor_HaveDistinctKeys()
    {
        Assert.NotEqual(PermissionType.Editor.Key, PermissionType.Owner.Key);
    }

    [Fact]
    public void GetByKey_ReturnsOwner_ForOwnerKey()
    {
        var result = PermissionType.GetByKey<PermissionType>(PermissionType.Owner.Key);

        Assert.Equal(PermissionType.Owner, result);
    }

    [Fact]
    public void GetByKey_ReturnsEditor_ForEditorKey()
    {
        var result = PermissionType.GetByKey<PermissionType>(PermissionType.Editor.Key);

        Assert.Equal(PermissionType.Editor, result);
    }
}

using Andor.Communications.Domain.Users;
using PermissionType = Andor.Communications.Domain.ValueObjects.Type;

namespace Andor.Communications.Domain.Tests.Permissions;

public class PermissionNewTests
{
    private static Recipient CreateValidRecipient()
    {
        var (_, recipient) = Recipient.New("Jane Doe", "jane.doe@example.com", true, []);
        return recipient!;
    }

    [Fact]
    public void New_WithValidData_ShouldCreatePermissionSuccessfully()
    {
        // Arrange
        var recipient = CreateValidRecipient();

        // Act
        var (result, permission) = Permission.New(recipient, PermissionType.Marketing, consented: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(permission);
        Assert.Equal(recipient.Id, permission.RecipientId);
        Assert.Same(recipient, permission.Recipient);
        Assert.Equal(PermissionType.Marketing, permission.Type);
        Assert.True(permission.Consented);
    }

    [Fact]
    public void New_WithNullRecipient_ShouldThrowNullReferenceException()
    {
        // The private constructor dereferences recipient.Id before Validate() ever runs its
        // Recipient.NotNull() check, so a null recipient blows up instead of failing gracefully.
        Assert.Throws<NullReferenceException>(
            () => Permission.New(null!, PermissionType.Information, consented: false));
    }

    [Fact]
    public void New_WithConsentedFalse_ShouldPreserveConsentState()
    {
        // Arrange
        var recipient = CreateValidRecipient();

        // Act
        var (result, permission) = Permission.New(recipient, PermissionType.Information, consented: false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(permission);
        Assert.False(permission.Consented);
    }
}

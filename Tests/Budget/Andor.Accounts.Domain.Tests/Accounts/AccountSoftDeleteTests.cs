using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.PermissionTypes;
using Moq;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class AccountSoftDeleteTests
{
    private readonly Mock<IAccountValidator> _validatorMock;

    public AccountSoftDeleteTests()
    {
        _validatorMock = AccountFixture.CreateDefaultValidator();
    }

    #region SoftDelete - Success Cases

    [Fact]
    public async Task SoftDelete_WithOwnerPermission_ShouldMarkAccountAsDeleted()
    {
        // Arrange
        var (_, account) = await AccountFixture.CreateValidAccountAsync(_validatorMock);
        var ownerId = account!.Members.First(m => m.PermissionType == PermissionType.Owner).UserId;

        // Act
        var result = account.SoftDelete(ownerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(account.IsDeleted);
    }

    [Fact]
    public async Task SoftDelete_WithMultipleOwners_AnyOwnerCanDelete()
    {
        // Arrange
        var secondOwnerId = Guid.NewGuid();
        var (account, firstOwnerId) = await AccountFixture.CreateAccountWithMembersAsync(
            _validatorMock,
            [(secondOwnerId, PermissionType.Owner)]);

        // Act - Second owner deletes
        var result = account.SoftDelete(secondOwnerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(account.IsDeleted);
    }

    #endregion

    #region SoftDelete - Failure Cases

    [Fact]
    public async Task SoftDelete_WithEditorPermission_ShouldReturnFailure()
    {
        // Arrange
        var (account, _, editorUserId) = await AccountFixture.CreateAccountWithAdditionalMemberAsync(
            PermissionType.Editor,
            _validatorMock);

        // Act
        var result = account.SoftDelete(editorUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(account.IsDeleted);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.OnlyOwnerCanInviteMembers));
    }

    [Fact]
    public async Task SoftDelete_WithViewerPermission_ShouldReturnFailure()
    {
        // Arrange
        var (account, _, viewerUserId) = await AccountFixture.CreateAccountWithAdditionalMemberAsync(
            PermissionType.Viewer,
            _validatorMock);

        // Act
        var result = account.SoftDelete(viewerUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(account.IsDeleted);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.OnlyOwnerCanInviteMembers));
    }

    [Fact]
    public async Task SoftDelete_WithNonMemberUser_ShouldReturnFailure()
    {
        // Arrange
        var (_, account) = await AccountFixture.CreateValidAccountAsync(_validatorMock);
        var nonMemberUserId = Guid.NewGuid();

        // Act
        var result = account!.SoftDelete(nonMemberUserId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(account.IsDeleted);
        Assert.Contains(result.Errors, e =>
            e.Error.Equals(AccountErrorCode.UserNotMember));
    }

    #endregion
}

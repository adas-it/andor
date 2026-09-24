using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.DomainEvents;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Tests.Accounts;

public class RespondInviteTests
{
    [Fact]
    public async Task RespondInvite_WhenAccepted_AddsMemberWithInvitePermission()
    {
        var (_, account) = await AccountFixture.CreateValidAccountAsync();
        var ownerUserId = account!.Members.First().UserId;

        // Invite with Owner permission specifically: this is the scenario the PermissionType
        // key-collision bug would silently downgrade to Editor if it round-tripped through EF.
        var email = new Email("newowner@example.com");
        _ = account.InviteMemberByEmail(email, PermissionType.Owner, ownerUserId);

        var newUserId = UserId.New();
        _ = account.LinkUserToInvite(email, newUserId, ownerUserId);

        var inviteId = account.Invites.First().Id;
        var result = account.RespondInvite(inviteId, newUserId.Value);

        Assert.True(result.IsSuccess);
        Assert.True(account.Invites.First().IsAccepted);
        Assert.Equal(2, account.Members.Count);

        var newMember = account.Members.First(m => m.UserId == newUserId.Value);
        Assert.Equal(PermissionType.Owner, newMember.PermissionType);
    }

    [Fact]
    public async Task RespondInvite_WhenUserAlreadyMember_ReturnsFailureAndDoesNotDuplicateOrMutateInvite()
    {
        var (_, account) = await AccountFixture.CreateValidAccountAsync();
        var ownerUserId = account!.Members.First().UserId;

        var email = new Email("newuser@example.com");
        _ = account.InviteMemberByEmail(email, PermissionType.Editor, ownerUserId);

        var newUserId = UserId.New();
        _ = account.LinkUserToInvite(email, newUserId, ownerUserId);

        var inviteId = account.Invites.First().Id;
        var firstResponse = account.RespondInvite(inviteId, newUserId.Value);
        Assert.True(firstResponse.IsSuccess);

        var secondResponse = account.RespondInvite(inviteId, newUserId.Value);

        Assert.True(secondResponse.IsFailure);
        Assert.Contains(secondResponse.Errors, e => e.Error == AccountErrorCode.UserAlreadyMember);
        Assert.Equal(2, account.Members.Count);
    }

    [Fact]
    public async Task RespondInvite_RaisesAccountMemberAddedDomainEvent()
    {
        var (_, account) = await AccountFixture.CreateValidAccountAsync();
        var ownerUserId = account!.Members.First().UserId;

        var email = new Email("newuser@example.com");
        _ = account.InviteMemberByEmail(email, PermissionType.Editor, ownerUserId);

        var newUserId = UserId.New();
        _ = account.LinkUserToInvite(email, newUserId, ownerUserId);

        var inviteId = account.Invites.First().Id;
        var result = account.RespondInvite(inviteId, newUserId.Value);

        Assert.True(result.IsSuccess);
        Assert.Contains(account.Events, e => e is AccountMemberAddedDomainEvent);
        Assert.Contains(account.Events, e => e is AccountMemberInviteAcceptedDomainEvent);
    }
}

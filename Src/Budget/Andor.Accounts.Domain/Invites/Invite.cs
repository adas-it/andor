using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Invites.Errors;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Invites;

public class Invite : Entity<InviteId>
{
    public AccountId AccountId { get; private set; }
    public UserId? UserId { get; private set; }
    public Email? Email { get; private set; }
    public PermissionType Permission { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAccepted { get; private set; }

    protected Invite()
    {
        Permission = PermissionType.Viewer;
        IsActive = true;
        IsAccepted = false;
    }

    private Invite(InviteId id, AccountId accountId, UserId? userId, Email? email, PermissionType permission)
    {
        Id = id;
        AccountId = accountId;
        UserId = userId;
        Email = email;
        Permission = permission;
        IsActive = true;
        IsAccepted = false;
    }
    public static (DomainResult, Invite?) NewForUser(
        InviteId id,
        AccountId accountId,
        UserId userId,
        PermissionType permission)
    {
        var entity = new Invite(id, accountId, userId, null, permission);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, Invite?) NewForUser(
        AccountId accountId,
        UserId userId,
        PermissionType permission)
        => NewForUser(InviteId.New(), accountId, userId, permission);
    public static (DomainResult, Invite?) NewForEmail(
        InviteId id,
        AccountId accountId,
        Email email,
        PermissionType permission)
    {
        var entity = new Invite(id, accountId, null, email, permission);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, Invite?) NewForEmail(
        AccountId accountId,
        Email email,
        PermissionType permission)
        => NewForEmail(InviteId.New(), accountId, email, permission);

    public DomainResult LinkUser(UserId userId)
    {
        if (UserId.HasValue)
        {
            AddNotification(nameof(UserId), InviteErrorMessages.InviteAlreadyLinkedToUser, InviteErrorCode.InviteAlreadyLinkedToUser);
            return Validate();
        }

        if (!IsActive)
        {
            AddNotification(nameof(IsActive), InviteErrorMessages.InviteNotActive, InviteErrorCode.InviteNotActive);
            return Validate();
        }

        UserId = userId;
        return DomainResult.Success();
    }

    public DomainResult Accept()
    {
        if (!IsActive)
        {
            AddNotification(nameof(IsActive), InviteErrorMessages.InviteNotActive, InviteErrorCode.InviteNotActive);
            return Validate();
        }

        if (IsAccepted)
        {
            AddNotification(nameof(IsAccepted), InviteErrorMessages.InviteAlreadyAccepted, InviteErrorCode.InviteAlreadyAccepted);
            return Validate();
        }

        if (!UserId.HasValue)
        {
            AddNotification(nameof(UserId), InviteErrorMessages.InviteNotLinkedToUser, InviteErrorCode.InviteNotLinkedToUser);
            return Validate();
        }

        IsAccepted = true;
        IsActive = false;
        return DomainResult.Success();
    }

    public DomainResult Reject()
    {
        if (!IsActive)
        {
            AddNotification(nameof(IsActive), InviteErrorMessages.InviteNotActive, InviteErrorCode.InviteNotActive);
            return Validate();
        }

        if (IsAccepted)
        {
            AddNotification(nameof(IsAccepted), InviteErrorMessages.CannotRejectAcceptedInvite, InviteErrorCode.CannotRejectAcceptedInvite);
            return Validate();
        }

        IsActive = false;
        return DomainResult.Success();
    }

    public DomainResult Deactivate()
    {
        IsActive = false;
        return DomainResult.Success();
    }

    protected override DomainResult Validate()
    {
        AddNotification(AccountId.NotNull());

        // Deve ter Email OU UserId preenchido (pelo menos um)
        if (!UserId.HasValue && Email == null)
        {
            AddNotification(nameof(Email), InviteErrorMessages.InviteMustHaveEmailOrUserId, InviteErrorCode.InviteMustHaveEmailOrUserId);
        }

        return base.Validate();
    }
}


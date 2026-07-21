using Andor.Accounts.Domain.FinancialMovements;
using Andor.Foundation.Domain.Events;

namespace Andor.Accounts.Domain.Accounts.DomainEvents;

public sealed record AccountCreatedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountCreatedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountCreatedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountDeletedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountDeletedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountDeletedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountCategoryAddedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountCategoryAddedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountCategoryAddedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountSubCategoryAddedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountSubCategoryAddedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountSubCategoryAddedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountPaymentMethodAddedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountPaymentMethodAddedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountPaymentMethodAddedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountMemberAddedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountMemberAddedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountMemberAddedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountMemberRemovedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountMemberRemovedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountMemberRemovedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountMemberInvitedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountMemberInvitedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountMemberInvitedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountMemberInviteAcceptedDomainEvent : DomainEvent
{
    public string Name { get; init; }
    public Guid InviteId { get; init; }
    public Guid InvitedUserId { get; init; }

    public static AccountMemberInviteAcceptedDomainEvent FromAggregator(Account entity, Guid inviteId, Guid invitedUserId, Guid userId)
        => new AccountMemberInviteAcceptedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            InviteId = inviteId,
            InvitedUserId = invitedUserId,
            UserId = userId
        };
}
public sealed record AccountMemberInviteDeclinedDomainEvent : DomainEvent
{
    public string Name { get; init; }
    public Guid InviteId { get; init; }
    public Guid InvitedUserId { get; init; }

    public static AccountMemberInviteDeclinedDomainEvent FromAggregator(Account entity, Guid inviteId, Guid invitedUserId, Guid userId)
        => new AccountMemberInviteDeclinedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            InviteId = inviteId,
            InvitedUserId = invitedUserId,
            UserId = userId
        };
}
public sealed record AccountInviteUserLinkedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountInviteUserLinkedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountInviteUserLinkedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}
public sealed record AccountFinancialMovementAddedDomainEvent : DomainEvent
{
    public string Name { get; init; }
    public Guid FinancialMovementId { get; init; }
    public DateTime Date { get; init; }
    public string? Description { get; init; }
    public Guid SubCategoryId { get; init; }
    public Guid PaymentMethodId { get; init; }
    public decimal Value { get; init; }
    public int Status { get; init; }
    public int Type { get; init; }

    public static AccountFinancialMovementAddedDomainEvent FromAggregator(Account entity, FinancialMovement movement, Guid userId)
        => new AccountFinancialMovementAddedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId,
            FinancialMovementId = movement.Id,
            Date = movement.Date,
            Description = movement.Description,
            SubCategoryId = movement.SubCategoryId,
            PaymentMethodId = movement.PaymentMethodId,
            Value = movement.Value,
            Status = movement.Status.Key,
            Type = movement.Type.Key,
        };
}
public sealed record AccountFinancialMovementRemovedDomainEvent : DomainEvent
{
    public string Name { get; init; }

    public static AccountFinancialMovementRemovedDomainEvent FromAggregator(Account entity, Guid userId)
        => new AccountFinancialMovementRemovedDomainEvent() with
        {
            Id = entity.Id,
            Name = entity.Name!,
            UserId = userId
        };
}

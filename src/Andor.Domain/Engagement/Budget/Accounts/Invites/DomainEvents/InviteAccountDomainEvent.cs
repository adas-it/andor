namespace Andor.Domain.Engagement.Budget.Accounts.Invites.DomainEvents;

public sealed record InviteCreatedDomainEvent
{
    public Guid InviteId { get; init; }
    public Guid AccountId { get; init; }
    public Guid InvitingId { get; set; }
    public string Email { get; init; } = "";

    public static InviteCreatedDomainEvent FromAggregator(Invite entity)
        => new InviteCreatedDomainEvent() with
        {
            InviteId = entity.Id,
            Email = entity.Email,
            AccountId = entity.AccountId,
            InvitingId = entity.InvitingId,
        };
}
public sealed record GuestNotFoundDomainEvent
{
    public Guid InviteId { get; init; }
    public string Email { get; init; } = "";

    public static GuestNotFoundDomainEvent FromAggregator(Invite entity)
        => new GuestNotFoundDomainEvent() with
        {
            InviteId = entity.Id,
            Email = entity.Email
        };
}
public sealed record GuestFoundDomainEvent
{
    public Guid InviteId { get; init; }
    public Guid? GuestId { get; init; }

    public static GuestFoundDomainEvent FromAggregator(Invite invite)
        => new GuestFoundDomainEvent() with
        {
            InviteId = invite.Id,
            GuestId = invite.GuestId
        };
}
public sealed record InvitationMadeDomainEvent
{
    public Guid InviteId { get; init; }

    public static InvitationMadeDomainEvent FromAggregator(Invite invite)
        => new InvitationMadeDomainEvent() with
        {
            InviteId = invite.Id
        };
}
public sealed record InvitationAnsweredDomainEvent
{
    public Guid InviteId { get; init; }
    public KeyValuePair<int, string> Answer { get; init; }

    public static InvitationAnsweredDomainEvent FromAggregator(Invite invite)
        => new InvitationAnsweredDomainEvent() with
        {
            InviteId = invite.Id,
            Answer = new KeyValuePair<int, string>(invite.Status.Key, invite.Status.Name)
        };
}
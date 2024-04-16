namespace Andor.Domain.Engagement.Budget.Entities.Invites.DomainEvents;

public sealed record InviteCreatedDomainEvent
{
    public Guid Id { get; init; }
    public string Email { get; init; } = "";

    public static InviteCreatedDomainEvent FromAggregator(Invite entity)
        => new InviteCreatedDomainEvent() with
        {
            Id = entity.Id,
            Email = entity.Email.Address
        };
}
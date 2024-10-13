using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using MassTransit;

namespace Andor.Application.Engagement.Budget.Invites.Saga
{
    public record InviteSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public InviteId InviteId { get; set; }
        public UserId InvitingId { get; set; }
        public UserId? GuestId { get; set; }
        public string GuestEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

    }
}

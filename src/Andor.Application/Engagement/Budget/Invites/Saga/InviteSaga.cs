using Andor.Domain.Engagement.Budget.Accounts.Invites.DomainEvents;
using MassTransit;

namespace Andor.Application.Engagement.Budget.Invites.Saga
{
    public class InviteSaga : MassTransitStateMachine<InviteSagaState>
    {
        public State InviteCreated { get; private set; }
        public State GuestNotFound { get; private set; }
        public State GuestFound { get; private set; }
        public State InvitationMade { get; private set; }
        public State InvitationAnswered { get; private set; }
        public Event<InviteCreatedDomainEvent> InviteCreatedDomainEvent { get; private set; }
        public Event<GuestNotFoundDomainEvent> GuestNotFoundDomainEvent { get; private set; }
        public Event<GuestFoundDomainEvent> GuestFoundDomainEvent { get; private set; }
        public Event<InvitationMadeDomainEvent> InvitationMadeDomainEvent { get; private set; }
        public Event<InvitationAnsweredDomainEvent> InvitationAnsweredDomainEvent { get; private set; }

        public InviteSaga()
        {
            InstanceState(x => x.Status);

            Event(() => InviteCreatedDomainEvent, x => x.CorrelateById(m => m.Message.InviteId));
            Event(() => GuestNotFoundDomainEvent, x => x.CorrelateById(m => m.Message.InviteId));
            Event(() => GuestFoundDomainEvent, x => x.CorrelateById(m => m.Message.InviteId));
            Event(() => InvitationMadeDomainEvent, x => x.CorrelateById(m => m.Message.InviteId));
            Event(() => InvitationAnsweredDomainEvent, x => x.CorrelateById(m => m.Message.InviteId));

            Initially(
                When(InviteCreatedDomainEvent)
                    .Then(context =>
                    {
                        context.Instance.GuestEmail = context.Data.Email;
                        context.Instance.InvitingId = context.Data.InvitingId;
                        context.Instance.InviteId = context.Data.InviteId;
                    })
                    .TransitionTo(InviteCreated));

            During(InviteCreated,
                When(GuestFoundDomainEvent)
                    .Then(context =>
                    {
                        context.Instance.GuestId = context.Data.GuestId;
                    })
                    .TransitionTo(GuestFound));

            During(InviteCreated,
                When(GuestNotFoundDomainEvent)
                    .TransitionTo(GuestNotFound));

            During(GuestNotFound,
                When(GuestFoundDomainEvent)
                    .Then(context =>
                    {
                        context.Instance.GuestId = context.Data.GuestId;
                    })
                    .TransitionTo(GuestFound));


            During(GuestFound,
                When(InvitationMadeDomainEvent)
                    .TransitionTo(InvitationMade));


            During(InvitationMade,
                When(InvitationAnsweredDomainEvent)
                    .Finalize());
        }
    }
}

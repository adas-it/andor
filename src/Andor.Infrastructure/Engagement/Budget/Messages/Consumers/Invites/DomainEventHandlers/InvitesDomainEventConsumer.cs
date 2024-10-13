using Andor.Application.Engagement.Budget.Invites.Commands;
using Andor.Application.Engagement.Budget.Invites.DomainEventHandlers;
using Andor.Domain.Engagement.Budget.Accounts.Invites.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Users.DomainEvents;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Invites.DomainEventHandlers;

public class InvitesDomainEventConsumer(IMediator _mediator) :
    IConsumer<InviteCreatedDomainEvent>,
    IConsumer<GuestNotFoundDomainEvent>,
    IConsumer<GuestFoundDomainEvent>,
    IConsumer<InvitationMadeDomainEvent>,
    IConsumer<InvitationAnsweredDomainEvent>,
    IConsumer<UserCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<InviteCreatedDomainEvent> context)
    {
        var command = new InviteCreatedCommand(context.Message.InviteId, new System.Net.Mail.MailAddress(context.Message.Email));

        await _mediator.Send(command);
    }
    public async Task Consume(ConsumeContext<GuestNotFoundDomainEvent> context)
    {
        var command = new GuestNotFoundCommand(context.Message.InviteId);

        await _mediator.Send(command);
    }
    public async Task Consume(ConsumeContext<GuestFoundDomainEvent> context)
    {
        var command = new GuestFoundCommand(context.Message.InviteId);
        await _mediator.Send(command);
    }
    public async Task Consume(ConsumeContext<InvitationMadeDomainEvent> context)
    {
        var command = new InvitationMadeCommand(context.Message.InviteId);
        await _mediator.Send(command);
    }
    public async Task Consume(ConsumeContext<InvitationAnsweredDomainEvent> context)
    {
        var command = new InvitationAnsweredCommand(context.Message.InviteId);
        await _mediator.Send(command);
    }

    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        var command = new CheckUserPendingToInviteCommand()
        {
            UserId = context.Message.Id,
            Email = context.Message.Email
        };

        await _mediator.Send(command);
    }
}

using Andor.Application.Engagement.Budget.Accounts.Commands;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Invites.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Users.DomainEvents;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Engagement.Budget.Messages.Consumers.Accounts.DomainEventHandlers;

public class UserDomainEventConsumer(IMediator _mediator) :
    IConsumer<FinancialMovementDeletedDomainEvent>,
    IConsumer<FinancialMovementChangedDomainEvent>,
    IConsumer<FinancialMovementCreatedDomainEvent>,
    IConsumer<InviteCreatedDomainEvent>,
    IConsumer<GuestNotFoundDomainEvent>,
    IConsumer<GuestFoundDomainEvent>,
    IConsumer<InvitationMadeDomainEvent>,
    IConsumer<InvitationAnsweredDomainEvent>,
    IConsumer<UserCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        var command = new CreateAccountCommand()
        {
            CurrencyId = context.Message.PreferredCurrencyId,
            UserId = context.Message.Id,
            AccountName = $"{context.Message.FirstName}´s Account"
        };

        await _mediator.Send(command);
    }

    public Task Consume(ConsumeContext<FinancialMovementDeletedDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<FinancialMovementChangedDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<FinancialMovementCreatedDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<InviteCreatedDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<GuestNotFoundDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<GuestFoundDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<InvitationMadeDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<InvitationAnsweredDomainEvent> context)
    {
        //DUMMY

        return Task.CompletedTask;
    }
}

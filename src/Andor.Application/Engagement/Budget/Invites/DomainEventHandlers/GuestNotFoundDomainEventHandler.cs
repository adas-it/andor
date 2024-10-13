using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record GuestNotFoundCommand(InviteId InviteId) : IRequest
    {
    }

    internal class GuestNotFoundDomainEventHandler(ICommandsInviteRepository inviteRepository)
    : IRequestHandler<GuestNotFoundCommand>
    {
        public async Task Handle(GuestNotFoundCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            if (invite == null)
            {
                throw new ArgumentNullException(nameof(invite));
            }
        }
    }
}

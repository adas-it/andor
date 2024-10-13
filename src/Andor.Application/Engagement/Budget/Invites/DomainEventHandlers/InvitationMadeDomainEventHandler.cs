using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record InvitationMadeCommand(InviteId InviteId) : IRequest
    {
    }

    internal class InvitationMadeDomainEventHandler(ICommandsInviteRepository inviteRepository)
    : IRequestHandler<InvitationMadeCommand>
    {
        public async Task Handle(InvitationMadeCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            if (invite == null)
            {
                throw new ArgumentNullException(nameof(invite));
            }
        }
    }
}

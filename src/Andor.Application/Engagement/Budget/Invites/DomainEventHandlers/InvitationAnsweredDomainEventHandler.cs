using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record InvitationAnsweredCommand(InviteId InviteId) : IRequest
    {
    }

    internal class InvitationAnsweredDomainEventHandler(ICommandsInviteRepository inviteRepository)
    : IRequestHandler<InvitationAnsweredCommand>
    {
        public async Task Handle(InvitationAnsweredCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            if (invite == null)
            {
                throw new ArgumentNullException(nameof(invite));
            }
        }
    }
}

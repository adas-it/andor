using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record GuestFoundCommand(InviteId InviteId) : IRequest
    {
    }

    internal class GuestFoundDomainEventHandler(ICommandsInviteRepository inviteRepository,
        IUnitOfWork unitOfWork)
    : IRequestHandler<GuestFoundCommand>
    {
        public async Task Handle(GuestFoundCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            if (invite == null)
            {
                throw new ArgumentNullException(nameof(invite));
            }

            invite.InvitationMade();

            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}

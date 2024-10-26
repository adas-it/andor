using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record InvitationAnsweredCommand(InviteId InviteId) : IRequest
    {
    }

    internal class InvitationAnsweredDomainEventHandler(
        ICommandsInviteRepository inviteRepository,
        IUnitOfWork unitOfWork)
    : IRequestHandler<InvitationAnsweredCommand>
    {
        public async Task Handle(InvitationAnsweredCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            _ = invite ?? throw new ArgumentNullException(nameof(invite));
            _ = invite.Account ?? throw new ArgumentNullException(nameof(invite.Account));
            _ = invite.Guest ?? throw new ArgumentNullException(nameof(invite.Guest));

            if (invite.Status == Domain.Engagement.Budget.Accounts.Invites.InviteStatus.Accepted)
            {
                if (invite.Account.Users.Any(x => x.UserId == invite.GuestId))
                {
                    return;
                }

                invite.Account.AddNewMember(invite.Guest);

                await unitOfWork.CommitAsync(cancellationToken);
            }
        }
    }
}

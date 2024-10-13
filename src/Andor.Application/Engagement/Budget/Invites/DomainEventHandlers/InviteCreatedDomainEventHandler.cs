using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Users.Repositories;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record InviteCreatedCommand(InviteId InviteId, MailAddress guestMail) : IRequest
    {
    }

    internal class InviteCreatedDomainEventHandler(ICommandsUserRepository userRepository,
        ICommandsInviteRepository inviteRepository,
        IUnitOfWork unitOfWork)
    : IRequestHandler<InviteCreatedCommand>
    {
        public async Task Handle(InviteCreatedCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            if (invite == null)
            {
                throw new ArgumentNullException(nameof(invite));
            }

            var guest = await userRepository.GetByMailAsync(request.guestMail, cancellationToken);

            if (guest != null)
            {
                invite.GuestFound(guest.Id);
            }
            else
            {
                invite.GuestNotFound();
            }

            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}

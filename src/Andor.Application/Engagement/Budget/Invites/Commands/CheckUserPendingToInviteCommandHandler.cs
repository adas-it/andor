using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.Commands;

public record CheckUserPendingToInviteCommand : IRequest
{
    public string Email { get; set; }
    public UserId UserId { get; set; }
}

public class CheckUserPendingToInviteCommandHandler(
    ICommandsInviteRepository _inviteRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<CheckUserPendingToInviteCommand>
{
    [Log]
    [Transaction]
    public async Task Handle(CheckUserPendingToInviteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var collection = await _inviteRepository.GetAllPendingByGuestEmailAsync(request.Email, cancellationToken);

            foreach (var entity in collection)
            {
                entity.GuestFound(request.UserId);
            }

            if (collection.Any())
            {
                await _unitOfWork.CommitAsync(cancellationToken);
            }

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
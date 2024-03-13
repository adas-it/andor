namespace Family.Budget.Application.Accounts.Commands;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Accounts.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Accounts.Exceptions;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public record ShareCommand : IRequest<Unit?>
{
    public AccountId AccountId { get; set; }
    public string Email { get; set; }
}

public class ShareCommandHandler : IRequestHandler<ShareCommand, Unit?>
{
    private readonly IAccountRepository _db;
    private readonly IUnitOfWork _unitOfwork;
    private readonly Notifier _notifier;

    public ShareCommandHandler(IAccountRepository db,
        Notifier notifier,
        IUnitOfWork unitOfwork)
    {
        _db = db;
        _notifier = notifier;
        _unitOfwork = unitOfwork;
    }

    public async Task<Unit?> Handle(ShareCommand request, CancellationToken cancellation)
    {
        var account = await _db.GetById(request.AccountId, cancellation);

        if (account == null)
        {
            _notifier.Erros.Add(AccountError.AccountNotFound());
            return null;
        }

        try
        {
            account!.InviteAFriend(request.Email);

            await _db.Update(account, cancellation);

            await _unitOfwork.CommitAsync(cancellation);

            return Unit.Value;
        }
        catch (UserAlreadInvitedToAccount)
        {
            _notifier.Warnings.Add(AccountError.UserAlreadAddedToAccount());
            throw;
        }
    }
}

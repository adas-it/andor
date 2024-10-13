using Andor.Application.Common.Interfaces;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.DomainEventHandlers;

public record FinancialMovementManagedAccountsCommand(AccountId AccountId) : IRequest
{
}

internal class FinancialMovementManagedAccountsCommandHandler(
        IUnitOfWork _unitOfWork,
        IQueriesFinancialMovementRepository _financialMovementRepository,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<FinancialMovementManagedAccountsCommand>
{
    public async Task Handle(FinancialMovementManagedAccountsCommand request, CancellationToken cancellationToken)
    {
        var _account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        var firstFinancialMovement = await _financialMovementRepository.GetFirstMovement(request.AccountId, cancellationToken);
        var lastFinancialMovement = await _financialMovementRepository.GetLastMovement(request.AccountId, cancellationToken);

        _account.SetFirstMovement(firstFinancialMovement);
        _account.SetLastMovement(lastFinancialMovement);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
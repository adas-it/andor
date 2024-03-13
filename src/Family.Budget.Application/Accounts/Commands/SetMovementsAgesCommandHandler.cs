using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using MediatR;

namespace Family.Budget.Application.Accounts.Commands;

public record SetMovementsAgesCommandHandlerCommand : IRequest<Unit>
{
    public FinancialMovement Entity { get; set; }
}

public class SetMovementsAgesCommandHandler : BaseCommands, IRequestHandler<SetMovementsAgesCommandHandlerCommand, Unit>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IFinancialMovementRepository _financialMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetMovementsAgesCommandHandler(Notifier notifier, IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
        IFinancialMovementRepository financialMovementRepository) : base(notifier)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _financialMovementRepository = financialMovementRepository;
    }


    public async Task<Unit> Handle(SetMovementsAgesCommandHandlerCommand request, CancellationToken cancellationToken)
    {
        var financialMoviment = request.Entity;

        var account = await _accountRepository.GetById(financialMoviment.AccountId, cancellationToken);

        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var firstMovement = await _financialMovementRepository.GetFirstMovement(financialMoviment.AccountId, cancellationToken);
        var lastMovement = await _financialMovementRepository.GetLastMovement(financialMoviment.AccountId, cancellationToken);
        
        account.SetFirstMovement(firstMovement ?? DateTimeOffset.UtcNow);
        account.SetLastMovement(lastMovement ?? DateTimeOffset.UtcNow);
        
        await _accountRepository.Update(account, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}

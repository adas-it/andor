namespace Family.Budget.Application.Accounts.DomainEventsHandler;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.DomainEvents;
using MediatR;
using System.Threading.Tasks;

public class FinancialMovementCreatedDomainEventHandler :
    INotificationHandler<FinancialMovementCreatedDomainEvent>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FinancialMovementCreatedDomainEventHandler(IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(FinancialMovementCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var financialMoviment = notification.Entity;

        var account = await _accountRepository.GetById(financialMoviment.AccountId, cancellationToken);

        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        if (account.FirstMovement >= financialMoviment.Date)
        {
            account.SetFirstMovement(financialMoviment.Date);
        }

        if (account.LastMovement <= financialMoviment.Date)
        {
            account.SetLastMovement(financialMoviment.Date);
        }

        await _accountRepository.Update(account, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
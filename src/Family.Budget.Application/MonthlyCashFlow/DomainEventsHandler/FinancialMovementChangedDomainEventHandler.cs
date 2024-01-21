namespace Family.Budget.Application.MonthlyCashFlow.DomainEventsHandler;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.DomainEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal class FinancialMovementChangedDomainEventHandler :
    INotificationHandler<FinancialMovementChangedDomainEvent>
{
    private readonly ICashFlowRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FinancialMovementChangedDomainEventHandler(ICashFlowRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(FinancialMovementChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var year = notification.Entity.Date.Year;
        var month = notification.Entity.Date.Month;
        var accountId = notification.Entity.AccountId;

        var cashFlow = await _repository.GetByAccountIdAsync(accountId, year, month, cancellationToken);

        if (cashFlow == null)
        {
            var previus = await _repository.GetPreviousCashFlowByAccountIdAsync(accountId, year, month, cancellationToken);

            cashFlow = CashFlow.New(year, month, accountId, previus?.AccountBalance ?? 0);

            cashFlow!.AddFinancialMovement(notification.Entity);

            await _repository.Insert(cashFlow, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        cashFlow!.UpdateFinancialMovement(notification.OldEntity, notification.Entity);

        await _repository.Update(cashFlow, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

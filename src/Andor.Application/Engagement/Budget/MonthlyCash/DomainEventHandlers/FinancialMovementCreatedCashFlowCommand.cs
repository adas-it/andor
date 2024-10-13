using Andor.Application.Common.Interfaces;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.DomainEventHandlers;

public record FinancialMovementCreatedCashFlowCommand(FinancialMovementCreatedDomainEvent context) : IRequest
{
}

public class FinancialMovementCreatedCashFlowCommandHandler(
        IUnitOfWork _unitOfWork,
        ICommandsCashFlowRepository _cashFlowRepository,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<FinancialMovementCreatedCashFlowCommand>
{
    public async Task Handle(FinancialMovementCreatedCashFlowCommand request, CancellationToken cancellationToken)
    {
        var current = request.context.Current;

        var _month = Month.Load(current.Date.Month);
        var _year = Year.Load(current.Date.Year);

        var _type = MovementType.GetByKey<MovementType>(current.Type);
        var _status = MovementStatus.GetByKey<MovementStatus>(current.Status);

        var _account = await _accountRepository.GetByIdAsync(current.AccountId, cancellationToken);

        var _cashFlow = await _cashFlowRepository.GetCurrentOrPreviousCashFlowAsync(
            _account.Id, _year, _month, cancellationToken);

        CashFlow cashFlow;

        if (_cashFlow == null || _cashFlow.Month != _month || _cashFlow.Year != _year)
        {
            (_, cashFlow) = CashFlow.New(
                _year,
                _month,
                _account,
                _cashFlow?.AccountBalance ?? 0);

            await _cashFlowRepository.InsertAsync(cashFlow, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            cashFlow = _cashFlow;
        }

        cashFlow.AddFinancialMovement(_type, _status, current.Value);

        await _cashFlowRepository.UpdateAsync(cashFlow, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
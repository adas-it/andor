using Andor.Application.Common.Interfaces;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.DomainEventHandlers;

public record FinancialMovementChangedCashFlowCommand(FinancialMovementChangedDomainEvent context) : IRequest
{
}

public class FinancialMovementChangedCashFlowCommandHandler(
        IUnitOfWork _unitOfWork,
        ICommandsCashFlowRepository _cashFlowRepository,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<FinancialMovementChangedCashFlowCommand>
{
    public async Task Handle(FinancialMovementChangedCashFlowCommand request, CancellationToken cancellationToken)
    {
        var current = request.context.Current;
        var old = request.context.Old;

        var _account = await _accountRepository.GetByIdAsync(current.AccountId, cancellationToken);

        var _month = Month.Load(current.Date.Month);
        var _year = Year.Load(current.Date.Year);

        var _oldMonth = Month.Load(old.Date.Month);
        var _oldYear = Year.Load(old.Date.Year);

        var _type = MovementType.GetByKey<MovementType>(current.Type);
        var _status = MovementStatus.GetByKey<MovementStatus>(current.Status);

        var _oldType = MovementType.GetByKey<MovementType>(old.Type);
        var _oldStatus = MovementStatus.GetByKey<MovementStatus>(old.Status);


        var _cashFlow = await _cashFlowRepository
            .GetCurrentOrPreviousCashFlowAsync(_account.Id, _year, _month, cancellationToken);

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

        if (_month == _oldMonth && _year == _oldYear)
        {
            cashFlow.AddFinancialMovement(_oldType, _oldStatus, -old.Value);
        }
        else
        {
            CashFlow _oldCashFlow;

            _oldCashFlow = await _cashFlowRepository
                .GetCurrentOrPreviousCashFlowAsync(_account.Id, _oldYear, _oldMonth, cancellationToken);

            _oldCashFlow.AddFinancialMovement(_oldType, _oldStatus, -old.Value);

            await _cashFlowRepository.UpdateAsync(_oldCashFlow, cancellationToken);
        }

        cashFlow.AddFinancialMovement(_type, _status, current.Value);

        await _cashFlowRepository.UpdateAsync(cashFlow, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
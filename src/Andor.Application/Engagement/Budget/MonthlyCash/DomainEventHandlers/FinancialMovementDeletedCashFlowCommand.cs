using Andor.Application.Common.Interfaces;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.DomainEventHandlers;

public record FinancialMovementDeletedCashFlowCommand(FinancialMovementDeletedDomainEvent context) : IRequest
{
}

public class FinancialMovementDeletedCashFlowCommandHandler(
        IUnitOfWork _unitOfWork,
        ICommandsCashFlowRepository _cashFlowRepository,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<FinancialMovementDeletedCashFlowCommand>
{
    public async Task Handle(FinancialMovementDeletedCashFlowCommand request, CancellationToken cancellationToken)
    {
        if (request.context.Current.IsItCreditHandling)
        {
            return;
        }

        var current = request.context.Current;

        var _account = await _accountRepository.GetByIdAsync(current.AccountId, cancellationToken);

        var _month = Month.Load(current.Date.Month);
        var _year = Year.Load(current.Date.Year);

        var _type = MovementType.GetByKey<MovementType>(current.Type);
        var _status = MovementStatus.GetByKey<MovementStatus>(current.Status);

        var _cashFlow = await _cashFlowRepository
            .GetCurrentOrPreviousCashFlowAsync(_account.Id, _year, _month, cancellationToken);

        if (_cashFlow != null && _cashFlow.Month == _month && _cashFlow.Year == _year)
        {
            _cashFlow.AddFinancialMovement(_type, _status, -current.Value);

            await _cashFlowRepository.UpdateAsync(_cashFlow, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
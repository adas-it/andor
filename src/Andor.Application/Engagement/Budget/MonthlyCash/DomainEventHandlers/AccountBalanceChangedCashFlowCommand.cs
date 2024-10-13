using Andor.Application.Common.Interfaces;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.DomainEvents;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.DomainEventHandlers;

public record AccountBalanceChangedCashFlowCommand(AccountBalanceChangedDomainEvent context) : IRequest
{
}

public class AccountBalanceChangedCashFlowCommandHandler(
        IUnitOfWork _unitOfWork,
        ICommandsCashFlowRepository _cashFlowRepository) : IRequestHandler<AccountBalanceChangedCashFlowCommand>
{
    public async Task Handle(AccountBalanceChangedCashFlowCommand request, CancellationToken cancellationToken)
    {
        var current = request.context.Current;

        if (current.Month == 12)
        {
            current.Year += 1;
            current.Month = 1;
        }
        else
        {
            current.Month += 1;
        }

        var cashFlow = await _cashFlowRepository.GetCurrentOrNextCashFlowAsync((AccountId)current.AccountId, (Year)current.Year, (Month)current.Month, cancellationToken);

        if (cashFlow != null)
        {
            cashFlow.SetFinalBalancePreviousMonth(current.AccountBalance);

            await _cashFlowRepository.UpdateAsync(cashFlow, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
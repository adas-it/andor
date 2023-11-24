using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Family.Budget.Domain.SeedWork;
using MediatR;

namespace Family.Budget.Application.MonthlyCashFlow.Commands;

public record SetNewFinalBalanceByChangedAccountBalanceCommand : IRequest<Unit>
{
    public CashFlow Entity { get; set; }
}

public class SetNewFinalBalanceByChangedAccountBalanceCommandHandler : BaseCommands, IRequestHandler<SetNewFinalBalanceByChangedAccountBalanceCommand, Unit>
{
    private readonly ICashFlowRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    public SetNewFinalBalanceByChangedAccountBalanceCommandHandler(Notifier notifier, ICashFlowRepository repository,
        IUnitOfWork unitOfWork) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SetNewFinalBalanceByChangedAccountBalanceCommand request, CancellationToken cancellationToken)
    {
        var year = request.Entity.Year;
        var month = request.Entity.Month;
        var accountId = request.Entity.AccountId;

        var latest = await _repository.GetNextCashFlowByAccountIdAsync(accountId, year, month, cancellationToken);

        if (latest == null)
        {
            return Unit.Value;
        }

        latest.SetFinalBalancePreviousMonth(request.Entity.AccountBalance);

        await _repository.Update(request.Entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}

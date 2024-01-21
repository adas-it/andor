using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using MediatR;

namespace Family.Budget.Application.MonthlyCashFlow.Commands;

public record CreateCashFlowToFillGapWithLatestOneCommand : IRequest<Unit>
{
    public CashFlow Entity { get; set; }
}

public class CreateCashFlowToFillGapWithLatestOneCommandHandler : BaseCommands, IRequestHandler<CreateCashFlowToFillGapWithLatestOneCommand, Unit>
{
    private readonly ICashFlowRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCashFlowToFillGapWithLatestOneCommandHandler(Notifier notifier, ICashFlowRepository repository,
        IUnitOfWork unitOfWork) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CreateCashFlowToFillGapWithLatestOneCommand request, CancellationToken cancellationToken)
    {
        var year = request.Entity.Year;
        var month = request.Entity.Month;
        var accountId = request.Entity.AccountId;

        var latest = await _repository.GetNextCashFlowByAccountIdAsync(accountId, year, month, cancellationToken);

        if (latest == null)
        {
            return Unit.Value;
        }

        var latestMonthLessOne = latest.Month - 1;
        var entityPlusOne = request.Entity.Month + 1;

        if ((latest.Year > request.Entity.Year) || (latest.Year == request.Entity.Year && latestMonthLessOne > request.Entity.Month))
        {
            for (var i = request.Entity.Year; i <= latest!.Year; i++)
            {
                if (latest.Year > i)
                {
                    for (var z = entityPlusOne; z <= 12; z++)
                    {
                        var tempCashFlow = CashFlow.New(i, z, accountId, latest?.AccountBalance ?? 0);

                        await _repository.Insert(tempCashFlow, cancellationToken);
                    }

                    entityPlusOne = 1;
                }

                if (latest!.Year == i)
                {
                    for (var z = entityPlusOne; z <= latestMonthLessOne; z++)
                    {
                        var tempCashFlow = CashFlow.New(i, z, accountId, latest?.AccountBalance ?? 0);

                        await _repository.Insert(tempCashFlow, cancellationToken);
                    }
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return Unit.Value;
    }
}

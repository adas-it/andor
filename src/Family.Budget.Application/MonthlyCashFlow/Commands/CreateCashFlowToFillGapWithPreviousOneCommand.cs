using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.CashFlow;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using MediatR;

namespace Family.Budget.Application.MonthlyCashFlow.Commands;

public record CreateCashFlowToFillGapWithPreviousOneCommand : IRequest<Unit>
{
    public CashFlow Entity { get; set; }
}

public class CreateCashFlowToFillGapWithPreviousOneCommandHandler : BaseCommands, IRequestHandler<CreateCashFlowToFillGapWithPreviousOneCommand, Unit>
{
    private readonly ICashFlowRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCashFlowToFillGapWithPreviousOneCommandHandler(Notifier notifier, ICashFlowRepository repository,
        IUnitOfWork unitOfWork) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CreateCashFlowToFillGapWithPreviousOneCommand request, CancellationToken cancellationToken)
    {
        var year = request.Entity.Year;
        var month = request.Entity.Month;
        var accountId = request.Entity.AccountId;

        var previous = await _repository.GetPreviousCashFlowByAccountIdAsync(accountId, year, month, cancellationToken);

        if (previous == null)
        {
            return Unit.Value;
        }

        var previousMonthPlusOne = previous.Month + 1;

        if ((previous.Year <= request.Entity.Year) || 
            (previous.Year == request.Entity.Year && previousMonthPlusOne < request.Entity.Month))
        {
            for (var i = previous!.Year; i <= request.Entity.Year; i++)
            {
                if (i < request.Entity.Year)
                {
                    for (var z = previousMonthPlusOne; z <= 12; z++)
                    {
                        var tempCashFlow = CashFlow.New(i, z, accountId, previous?.AccountBalance ?? 0);

                        await _repository.Insert(tempCashFlow, cancellationToken);
                    }

                    previousMonthPlusOne = 1;
                }

                if (i == request.Entity.Year)
                {
                    for (var z = previousMonthPlusOne; z < request.Entity.Month; z++)
                    {
                        var tempCashFlow = CashFlow.New(i, z, accountId, previous?.AccountBalance ?? 0);

                        await _repository.Insert(tempCashFlow, cancellationToken);
                    }
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return Unit.Value;
    }
}

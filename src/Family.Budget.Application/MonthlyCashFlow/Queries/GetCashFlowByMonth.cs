namespace Family.Budget.Application.MonthlyCashFlow.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.CashFlows.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Common.ValuesObjects;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Mapster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public record GetCashFlowByMonthQuery : IRequest<CashFlowOutput>
{
    public AccountId AccountId { get; set; }
    public Year Year { get; set; }
    public Month Month { get; set; }
}

public class GetCashFlowByMonthQueryHandler : BaseCommands, IRequestHandler<GetCashFlowByMonthQuery, CashFlowOutput>
{
    private readonly ICashFlowRepository _cashFlowRepository;

    public GetCashFlowByMonthQueryHandler(ICashFlowRepository cashFlowRepository, Notifier notifier) : base(notifier)
    {
        _cashFlowRepository = cashFlowRepository;
    }

    public async Task<CashFlowOutput> Handle(GetCashFlowByMonthQuery request, CancellationToken cancellationToken)
    {
        var cashFlow = await _cashFlowRepository.GetCurrentOrPreviousCashFlowByAccountIdAsync(request.AccountId,
            request.Year, 
            request.Month, 
            cancellationToken);

        if(cashFlow == null)
        {
            return new CashFlowOutput();
        }

        if (cashFlow?.Month != request.Month)
        {
            return new CashFlowOutput()
            {
                Year = request.Year,
                Month = request.Month,
                FinalBalancePreviousMonth = cashFlow?.AccountBalance ?? 0,
                BalanceForecast = cashFlow?.AccountBalance ?? 0
            };
        }

        return cashFlow.Adapt<CashFlowOutput>();
    }
}
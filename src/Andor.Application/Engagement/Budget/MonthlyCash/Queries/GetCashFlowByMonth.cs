using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.MonthlyCash.Queries;

public record GetCashFlowByMonthQuery : IRequest<ApplicationResult<CashFlowOutput>>
{
    public AccountId AccountId { get; set; }
    public Year Year { get; set; } = DateTime.Now.Year;
    public Month Month { get; set; } = DateTime.Now.Month;
}

public class GetCashFlowByMonthQueryHandler(IQueriesCashFlowRepository _queriesCashFlow)
    : IRequestHandler<GetCashFlowByMonthQuery, ApplicationResult<CashFlowOutput>>
{
    public async Task<ApplicationResult<CashFlowOutput>> Handle(GetCashFlowByMonthQuery request, CancellationToken cancellationToken)
    {
        var result = ApplicationResult<CashFlowOutput>.Success();

        CashFlowOutput output;

        var cashFlow = await _queriesCashFlow.GetCurrentOrPreviousCashFlowByAccountIdAsync(
            request.AccountId,
            request.Year,
            request.Month,
            cancellationToken);

        if (cashFlow?.Month != request.Month)
        {
            output = new CashFlowOutput()
            {
                Year = request.Year,
                Month = request.Month,
                FinalBalancePreviousMonth = cashFlow?.AccountBalance ?? 0,
                BalanceForecast = cashFlow?.AccountBalance ?? 0
            };
        }
        else
        {
            output = cashFlow.Adapt<CashFlowOutput>();
        }

        result.SetData(output);

        return result;
    }
}

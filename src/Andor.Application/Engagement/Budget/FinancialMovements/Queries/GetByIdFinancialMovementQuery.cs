using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public class GetByIdFinancialMovementQuery : IRequest<ApplicationResult<FinancialMovementOutput>>
{
    public AccountId AccountId { get; set; }
    public FinancialMovementId FinancialMovementId { get; set; }
}

public class GetByIdFinancialMovementQueryHandler(IQueriesFinancialMovementRepository _repository)
    : IRequestHandler<GetByIdFinancialMovementQuery, ApplicationResult<FinancialMovementOutput>>
{
    public async Task<ApplicationResult<FinancialMovementOutput>> Handle(GetByIdFinancialMovementQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<FinancialMovementOutput>.Success();

        var item = await _repository.GetByIdAsync(request.FinancialMovementId, cancellationToken);

        response.SetData(item.Adapt<FinancialMovementOutput>());

        return response;
    }
}

using Family.Budget.Application.Dto.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Mapster;
using MediatR;

namespace Family.Budget.Application.MovementStatuses.Queries;


public class ListFinancialMovementStatusQuery
    : IRequest<List<FinancialMovementStatusOutput>>
{
}

public class ListFinancialMovementStatusesQueryHandler : IRequestHandler<ListFinancialMovementStatusQuery, List<FinancialMovementStatusOutput>>
{
    public Task<List<FinancialMovementStatusOutput>> Handle(ListFinancialMovementStatusQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            MovementStatus.GetAll<MovementStatus>().ToList().Adapt<List<FinancialMovementStatusOutput>>());
    }
}
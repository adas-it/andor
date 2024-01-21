namespace Family.Budget.Application.MovementTypes.Queries;

using Family.Budget.Application.Dto.MovementTypes;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Mapster;
using MediatR;

public class ListFinancialMovementTypeQuery
    : IRequest<List<MovementTypeOutput>>
{
}

public class ListFinancialMovementTypeQueryHandler : IRequestHandler<ListFinancialMovementTypeQuery, List<MovementTypeOutput>>
{
    public Task<List<MovementTypeOutput>> Handle(ListFinancialMovementTypeQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            MovementType.GetAll<MovementType>().ToList().Adapt<List<MovementTypeOutput>>());
    }
}
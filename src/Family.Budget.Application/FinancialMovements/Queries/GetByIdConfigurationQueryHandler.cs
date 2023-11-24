namespace Family.Budget.Application.FinancialMovements.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.FinancialMovements.ApplicationsErrors;
using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Mapster;
using MediatR;

public class GetByIdFinancialMovementQuery : IRequest<FinancialMovementOutput>
{
    public Guid Id { get; private set; }
    public GetByIdFinancialMovementQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdFinancialMovementQueryHandler : BaseCommands, IRequestHandler<GetByIdFinancialMovementQuery, FinancialMovementOutput>
{
    public IFinancialMovementRepository repository;

    public GetByIdFinancialMovementQueryHandler(IFinancialMovementRepository repository,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
    }

    public async Task<FinancialMovementOutput> Handle(GetByIdFinancialMovementQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.FinancialMovementNotFound());

            return null!;
        }

        return item.Adapt<FinancialMovementOutput>();
    }
}
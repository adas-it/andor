namespace Family.Budget.Application.FinancialMovements.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.FinancialMovements.ApplicationsErrors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using MediatR;

public record RemoveFinancialMovementCommand(Guid Id) : IRequest;

public class RemoveFinancialMovementCommandHandler : BaseCommands, IRequestHandler<RemoveFinancialMovementCommand>
{
    private readonly IFinancialMovementRepository repository;
    private readonly IUnitOfWork unitOfWork;

    public RemoveFinancialMovementCommandHandler(IFinancialMovementRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveFinancialMovementCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.FinancialMovementNotFound());
            return;
        }

        await repository.Delete(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return;
    }
}
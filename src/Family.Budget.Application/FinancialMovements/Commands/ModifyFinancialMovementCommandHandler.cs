namespace Family.Budget.Application.FinancialMovements.Commands;

using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.FinancialMovements.ApplicationsErrors;
using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PatchFinancialMovement : IRequest<FinancialMovementOutput>
{
    public JsonPatchDocument<ModifyFinancialMovementCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchFinancialMovement(Guid Id, JsonPatchDocument<ModifyFinancialMovementCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifyFinancialMovementCommandHandler : IRequestHandler<PatchFinancialMovement, FinancialMovementOutput>
{
    private readonly IFinancialMovementRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifyFinancialMovementCommandHandler(IFinancialMovementRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<FinancialMovementOutput> Handle(PatchFinancialMovement command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(ModifyFinancialMovementCommand.Value)}",
                    $"/{nameof(ModifyFinancialMovementCommand.Description)}",
                    $"/{nameof(ModifyFinancialMovementCommand.StatusId)}",
                    $"/{nameof(ModifyFinancialMovementCommand.Date)}",
                    $"/{nameof(ModifyFinancialMovementCommand.PaymentMethodId)}",
                    $"/{nameof(ModifyFinancialMovementCommand.SubCategoryId)}" }
                   );
        }
        catch (BusinessException ex)
        {
            notifier.Erros.Add(ex.ErrorCode);
            return null!;
        }

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            notifier.Erros.Add(Errors.FinancialMovementNotFound());
            return null!;
        }

        var oldItem = entity.Adapt<ModifyFinancialMovementCommand>();

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

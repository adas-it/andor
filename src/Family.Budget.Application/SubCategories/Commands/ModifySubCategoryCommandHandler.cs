namespace Family.Budget.Application.SubCategories.Commands;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.SubCategories.Errors;
using Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PatchSubCategory : IRequest<SubCategoryOutput>
{
    public JsonPatchDocument<ModifySubCategoryCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchSubCategory(Guid Id, JsonPatchDocument<ModifySubCategoryCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifySubCategoryCommandHandler : IRequestHandler<PatchSubCategory, SubCategoryOutput>
{
    private readonly ISubCategoryRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifySubCategoryCommandHandler(ISubCategoryRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<SubCategoryOutput> Handle(PatchSubCategory command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(ModifySubCategoryCommand.Name)}",
                    $"/{nameof(ModifySubCategoryCommand.Description)}",
                    $"/{nameof(ModifySubCategoryCommand.StartDate)}",
                    $"/{nameof(ModifySubCategoryCommand.DeactivationDate)}",
                    $"/{nameof(ModifySubCategoryCommand.CategoryId)}" }
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
            notifier.Erros.Add(Errors.SubCategoryNotFound());
            return null!;
        }

        var oldItem = new ModifySubCategoryCommand(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.StartDate,
            entity.DeactivationDate,
            entity.Category.Id);

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

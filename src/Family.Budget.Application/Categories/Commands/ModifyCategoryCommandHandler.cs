namespace Family.Budget.Application.Categories.Commands;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Categories.Errors;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Categories.Repository;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PatchCategory : IRequest<CategoryOutput>
{
    public JsonPatchDocument<ModifyCategoryCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchCategory(Guid Id, JsonPatchDocument<ModifyCategoryCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifyCategoryCommandHandler : IRequestHandler<PatchCategory, CategoryOutput>
{
    private readonly ICategoryRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifyCategoryCommandHandler(ICategoryRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<CategoryOutput> Handle(PatchCategory command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(Category.Name)}",
                    $"/{nameof(Category.Description)}",
                    $"/{nameof(Category.StartDate)}",
                    $"/{nameof(Category.DeactivationDate)}" }
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
            notifier.Erros.Add(Errors.CategoryNotFound());
            return null!;
        }

        var oldItem = new ModifyCategoryCommand(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.StartDate,
            entity.DeactivationDate,
            entity.Type.Key);

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

namespace Family.Budget.Application.Categories.Commands;

using Family.Budget.Application;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Categories.Errors;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangeCategoryCommandHandler : BaseCommands, IRequestHandler<ModifyCategoryCommand, CategoryOutput>
{
    private readonly ICategoryRepository repository;
    private readonly IUnitOfWork unitOfWork;

    public ChangeCategoryCommandHandler(ICategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<CategoryOutput> Handle(ModifyCategoryCommand command, CancellationToken cancellationToken)
    {
        CategoryOutput ret = null!;

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(Errors.CategoryNotFound());
            return null!;
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}

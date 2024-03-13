namespace Family.Budget.Application.SubCategories.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.SubCategories.Errors;
using Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangeSubCategoryCommandHandler : BaseCommands, IRequestHandler<ModifySubCategoryCommand, SubCategoryOutput>
{
    private readonly ISubCategoryRepository repository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IUnitOfWork unitOfWork;

    public ChangeSubCategoryCommandHandler(ISubCategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        ICategoryRepository categoryRepository) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.categoryRepository = categoryRepository;
    }

    public async Task<SubCategoryOutput> Handle(ModifySubCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetById(command.CategoryId, cancellationToken);

        if (category is null)
        {
            _notifier.Warnings.Add(Errors.CategoryNotFound());
            return null!;
        }

        SubCategoryOutput ret = null!;

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(Errors.SubCategoryNotFound());
            return null!;
        }

        entity.SetSubCategory(command.Name, command.Description, command.StartDate, command.DeactivationDate, category);

        await unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}

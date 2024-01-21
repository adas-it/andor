namespace Family.Budget.Application.Categories.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Categories.Services;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Categories.Requests;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Mapster;
using MediatR;

public record RegisterCategoryCommand : BaseCategory, IRequest<CategoryOutput>
{
    public RegisterCategoryCommand() { }
    public RegisterCategoryCommand(RegisterCategoryInput dto) : base(dto.Name, dto.Description, dto.StartDate, dto.DeactivationDate, dto.MovementTypeId)
    {
    }
}

public class RegisterCategoryCommandHandler : BaseCommands, IRequestHandler<RegisterCategoryCommand, CategoryOutput>
{
    private readonly ICategoryRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICategoryServices categoryServices;
    private readonly IMediator mediator;

    public RegisterCategoryCommandHandler(ICategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        ICategoryServices categoryServices,
        IMediator mediator) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.categoryServices = categoryServices;
        this.mediator = mediator;
    }

    public async Task<CategoryOutput> Handle(RegisterCategoryCommand request, CancellationToken cancellationToken)
    {
        var movimentType = MovementType.GetByKey<MovementType>(request.MovementTypeId);

        var item = Category.New(request.Name,
            request.Description,
            request.StartDate,
            request.DeactivationDate,
            movimentType!);

        await categoryServices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await repository.Insert(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<CategoryOutput>();
    }
}
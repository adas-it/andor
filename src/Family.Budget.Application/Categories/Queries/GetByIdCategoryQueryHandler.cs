namespace Family.Budget.Application.Categories.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.Categories.Errors;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories.Repository;
using Mapster;
using MediatR;

public class GetByIdCategoryQuery : IRequest<CategoryOutput>
{
    public Guid Id { get; private set; }
    public GetByIdCategoryQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdCategoryQueryHandler : BaseCommands, IRequestHandler<GetByIdCategoryQuery, CategoryOutput>
{
    public ICategoryRepository repository;

    public GetByIdCategoryQueryHandler(ICategoryRepository repository,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
    }

    public async Task<CategoryOutput> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.CategoryNotFound());

            return null!;
        }

        return item.Adapt<CategoryOutput>();
    }
}
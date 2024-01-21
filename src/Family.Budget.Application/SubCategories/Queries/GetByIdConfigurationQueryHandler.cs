namespace Family.Budget.Application.SubCategories.Queries;

using Family.Budget.Application;
using Family.Budget.Application.Dto.SubCategories.Errors;
using Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Mapster;
using MediatR;

public class GetByIdSubCategoryQuery : IRequest<SubCategoryOutput>
{
    public Guid Id { get; private set; }
    public GetByIdSubCategoryQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdSubCategoryQueryHandler : BaseCommands, IRequestHandler<GetByIdSubCategoryQuery, SubCategoryOutput>
{
    public ISubCategoryRepository repository;

    public GetByIdSubCategoryQueryHandler(ISubCategoryRepository repository,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
    }

    public async Task<SubCategoryOutput> Handle(GetByIdSubCategoryQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.SubCategoryNotFound());

            return null!;
        }

        return item.Adapt<SubCategoryOutput>();
    }
}
namespace Family.Budget.Application.SubCategories.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.SubCategories.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using MediatR;

public record RemoveSubCategoryCommand(Guid Id) : IRequest;

public class RemoveSubCategoryCommandHandler : BaseCommands, IRequestHandler<RemoveSubCategoryCommand>
{
    private readonly ISubCategoryRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFeatureFlagService featureFlagService;

    public RemoveSubCategoryCommandHandler(ISubCategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IFeatureFlagService featureFlagService) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.featureFlagService = featureFlagService;
    }

    public async Task Handle(RemoveSubCategoryCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.SubCategoryNotFound());
            return;
        }

        await repository.Delete(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return;
    }
}
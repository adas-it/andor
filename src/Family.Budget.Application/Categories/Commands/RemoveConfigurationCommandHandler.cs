namespace Family.Budget.Application.Categories.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Categories.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories.Repository;
using MediatR;

public record RemoveCategoryCommand(Guid Id) : IRequest;
public class RemoveCategoryCommandHandler : BaseCommands, IRequestHandler<RemoveCategoryCommand>
{
    private readonly ICategoryRepository repository;
    private readonly IUnitOfWork unitOfWork;

    public RemoveCategoryCommandHandler(ICategoryRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.CategoryNotFound());
            return;
        }

        await repository.Delete(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return;
    }
}
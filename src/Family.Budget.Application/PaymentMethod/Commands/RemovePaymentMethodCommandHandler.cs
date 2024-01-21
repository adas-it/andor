namespace Family.Budget.Application.PaymentMethod.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.PaymentMethods.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using MediatR;

public record RemovePaymentMethodCommand(Guid Id) : IRequest;

public class RemovePaymentMethodCommandHandler : BaseCommands, IRequestHandler<RemovePaymentMethodCommand>
{
    private readonly IPaymentMethodRepository repository;
    private readonly IUnitOfWork unitOfWork;

    public RemovePaymentMethodCommandHandler(IPaymentMethodRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(RemovePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.PaymentMethodNotFound());
            return;
        }

        await repository.Delete(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return;
    }
}
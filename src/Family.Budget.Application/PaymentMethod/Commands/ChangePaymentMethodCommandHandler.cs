namespace Family.Budget.Application.PaymentMethod.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.PaymentMethods.Errors;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangePaymentMethodCommandHandler : BaseCommands, IRequestHandler<ModifyPaymentMethodCommand, PaymentMethodOutput>
{
    private readonly IPaymentMethodRepository repository;
    private readonly IUnitOfWork unitOfWork;
    public ChangePaymentMethodCommandHandler(IPaymentMethodRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodOutput> Handle(ModifyPaymentMethodCommand command, CancellationToken cancellationToken)
    {
        PaymentMethodOutput ret = null!;

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(Errors.PaymentMethodNotFound());
            return null!;
        }

        await repository.Update(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}

namespace Family.Budget.Application.PaymentMethod.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.PaymentMethod.Services;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using MediatR;

public record RegisterPaymentMethodCommand : BasePaymentMethod, IRequest<PaymentMethodOutput>
{
}

public class RegisterPaymentMethodCommandHandler : BaseCommands, IRequestHandler<RegisterPaymentMethodCommand, PaymentMethodOutput>
{
    private readonly IPaymentMethodRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPaymentMethodServices PaymentMethodServices;

    public RegisterPaymentMethodCommandHandler(IPaymentMethodRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IPaymentMethodServices PaymentMethodServices) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.PaymentMethodServices = PaymentMethodServices;
    }

    public async Task<PaymentMethodOutput> Handle(RegisterPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var item = PaymentMethod.New(request.Name,
            request.Description,
            MovementType.MoneySpending,
            request.StartDate,
            request.DeactivationDate);

        await PaymentMethodServices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await repository.Insert(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return new PaymentMethodOutput(item.Id, item.Name, item.Description, item.StartDate, item.DeactivationDate);
    }
}
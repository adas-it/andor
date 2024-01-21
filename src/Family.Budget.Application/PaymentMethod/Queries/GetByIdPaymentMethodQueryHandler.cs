namespace Family.Budget.Application.PaymentMethod.Queries;
using MediatR;
using Family.Budget.Application.Dto.PaymentMethods.Errors;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Application;
using Family.Budget.Application.PaymentMethod.Adapters;

public class GetByIdPaymentMethodQuery : IRequest<PaymentMethodOutput>
{
    public Guid Id { get; private set; }
    public GetByIdPaymentMethodQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdPaymentMethodQueryHandler : BaseCommands, IRequestHandler<GetByIdPaymentMethodQuery, PaymentMethodOutput>
{
    public IPaymentMethodRepository repository;

    public GetByIdPaymentMethodQueryHandler(IPaymentMethodRepository repository,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
    }

    public async Task<PaymentMethodOutput> Handle(GetByIdPaymentMethodQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.PaymentMethodNotFound());

            return null!;
        }

        return item.MapDtoFromDomain();
    }
}
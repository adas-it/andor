namespace Family.Budget.Application.PaymentMethod.Commands;
using MediatR;
using Family.Budget.Application.Dto.PaymentMethods.Requests;
using Family.Budget.Application.Dto.PaymentMethods.Responses;


public record ModifyPaymentMethodCommand : Dto.PaymentMethods.Requests.BasePaymentMethod, IRequest<PaymentMethodOutput>
{
    public Guid Id { get; set; }
    public ModifyPaymentMethodCommand() : base() { }

    public ModifyPaymentMethodCommand(Guid id, ModifyPaymentMethodInput dto)
        : base(dto.Name, dto.Description, dto.StartDate, dto.DeactivationDate)
    {
        Id = id;
    }

    public ModifyPaymentMethodCommand(Guid id,
        string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate
        )
        : base(name, description, startDate, deactivationDate)
    {
        Id = id;
    }
}

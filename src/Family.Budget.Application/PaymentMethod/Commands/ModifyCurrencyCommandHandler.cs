namespace Family.Budget.Application.PaymentMethod.Commands;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.PaymentMethods.Errors;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PatchPaymentMethod : IRequest<PaymentMethodOutput>
{
    public JsonPatchDocument<ModifyPaymentMethodCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchPaymentMethod(Guid Id, JsonPatchDocument<ModifyPaymentMethodCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifyPaymentMethodCommandHandler : IRequestHandler<PatchPaymentMethod, PaymentMethodOutput>
{
    private readonly IPaymentMethodRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifyPaymentMethodCommandHandler(IPaymentMethodRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<PaymentMethodOutput> Handle(PatchPaymentMethod command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(PaymentMethod.Name)}",
                    $"/{nameof(PaymentMethod.Description)}",
                    $"/{nameof(PaymentMethod.StartDate)}",
                    $"/{nameof(PaymentMethod.DeactivationDate)}"
                   });
        }
        catch (BusinessException ex)
        {
            notifier.Erros.Add(ex.ErrorCode);
            return null!;
        }

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            notifier.Erros.Add(Errors.PaymentMethodNotFound());
            return null!;
        }

        var oldItem = new ModifyPaymentMethodCommand(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.StartDate,
            entity.DeactivationDate);

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

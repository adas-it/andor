namespace Family.Budget.Application.Currencies.Commands;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.Currencies.Errors;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Application.Common.Extensions;

public class PatchCurrency : IRequest<CurrencyOutput>
{
    public JsonPatchDocument<ModifyCurrencyCommand> PatchDocument { get; set; }
    public Guid Id { get; set; }

    public PatchCurrency(Guid Id, JsonPatchDocument<ModifyCurrencyCommand> PatchDocument)
    {
        this.PatchDocument = PatchDocument;
        this.Id = Id;
    }
}

public class ModifyCurrencyCommandHandler : IRequestHandler<PatchCurrency, CurrencyOutput>
{
    private readonly ICurrencyRepository repository;
    private readonly Notifier notifier;
    private readonly IMediator mediator;

    public ModifyCurrencyCommandHandler(ICurrencyRepository repository,
        Notifier notifier,
        IMediator mediator)
    {
        this.repository = repository;
        this.notifier = notifier;
        this.mediator = mediator;
    }

    public async Task<CurrencyOutput> Handle(PatchCurrency command, CancellationToken cancellationToken)
    {
        try
        {
            command.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(Currency.Name)}",
                    $"/{nameof(Currency.Iso)}" }
                   );
        }
        catch (BusinessException ex)
        {
            notifier.Erros.Add(ex.ErrorCode);
            return null!;
        }

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            notifier.Erros.Add(Errors.CurrencyNotFound());
            return null!;
        }

        var oldItem = new ModifyCurrencyCommand(
            entity.Id,
            entity.Name,
            entity.Iso);

        command.PatchDocument.ApplyTo(oldItem);

        var ret = await mediator.Send(oldItem, cancellationToken);

        return ret;
    }
}

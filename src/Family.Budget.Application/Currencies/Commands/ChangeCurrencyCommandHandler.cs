namespace Family.Budget.Application.Currencies.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Currencies.Services;
using Family.Budget.Application.Dto.Currencies.Errors;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangeCurrencyCommandHandler : BaseCommands, IRequestHandler<ModifyCurrencyCommand, CurrencyOutput>
{
    private readonly ICurrencyRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrencieservices Currencieservices;
    private readonly IMediator mediator;

    public ChangeCurrencyCommandHandler(ICurrencyRepository repository,
        IUnitOfWork unitOfWork,
        ICurrencieservices Currencieservices,
        IMediator mediator,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.Currencieservices = Currencieservices;
        this.mediator = mediator;
    }

    public async Task<CurrencyOutput> Handle(ModifyCurrencyCommand command, CancellationToken cancellationToken)
    {
        CurrencyOutput ret = null!;

        var entity = await repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(Errors.CurrencyNotFound());
            return null!;
        }

        entity.SetCurrency(command.Name, command.ISO);

        await repository.Update(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}

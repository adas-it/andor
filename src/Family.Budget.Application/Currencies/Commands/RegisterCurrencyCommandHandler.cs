namespace Family.Budget.Application.Currencies.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Currencies.Services;
using Family.Budget.Application.Dto.Currencies.Requests;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies;
using Family.Budget.Domain.Entities.Currencies.Repository;
using MediatR;

public record RegisterCurrencyCommand : BaseCurrency, IRequest<CurrencyOutput>
{
    public RegisterCurrencyCommand() { }
    public RegisterCurrencyCommand(RegisterCurrencyInput dto) : base(dto.Name, dto.ISO)
    {
    }
}

public class RegisterCurrencyCommandHandler : BaseCommands, IRequestHandler<RegisterCurrencyCommand, CurrencyOutput>
{
    private readonly ICurrencyRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrencieservices Currencieservices;
    private readonly IMediator mediator;

    public RegisterCurrencyCommandHandler(ICurrencyRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        ICurrencieservices Currencieservices,
        IMediator mediator) : base(notifier)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.Currencieservices = Currencieservices;
        this.mediator = mediator;
    }

    public async Task<CurrencyOutput> Handle(RegisterCurrencyCommand request, CancellationToken cancellationToken)
    {
        var item = Currency.New(request.Name,
            request.ISO, "$");

        await Currencieservices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await repository.Insert(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return new CurrencyOutput(item.Id, item.Name, item.Iso);
    }
}
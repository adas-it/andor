namespace Family.Budget.Application.Currencies.Queries;
using MediatR;
using Family.Budget.Application.Dto.Currencies.Errors;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Application;
using Family.Budget.Application.Currencies.Adapters;

public class GetByIdCurrencyQuery : IRequest<CurrencyOutput>
{
    public Guid Id { get; private set; }
    public GetByIdCurrencyQuery(Guid Id)
    {
        this.Id = Id;
    }
}

public class GetByIdCurrencyQueryHandler : BaseCommands, IRequestHandler<GetByIdCurrencyQuery, CurrencyOutput>
{
    public ICurrencyRepository repository;

    public GetByIdCurrencyQueryHandler(ICurrencyRepository repository,
        Notifier notifier) : base(notifier)
    {
        this.repository = repository;
    }

    public async Task<CurrencyOutput> Handle(GetByIdCurrencyQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.CurrencyNotFound());

            return null!;
        }

        return item.MapDtoFromDomain();
    }
}
namespace Family.Budget.Application.Currencies.Commands;
using MediatR;
using Family.Budget.Application.Dto.Currencies.Requests;
using Family.Budget.Application.Dto.Currencies.Responses;


public record ModifyCurrencyCommand : Dto.Currencies.Requests.BaseCurrency, IRequest<CurrencyOutput>
{
    public Guid Id { get; set; }
    public ModifyCurrencyCommand() : base() { }

    public ModifyCurrencyCommand(Guid id, ModifyCurrencyInput dto)
        : base(dto.Name, dto.ISO)
    {
        Id = id;
    }

    public ModifyCurrencyCommand(Guid id, string name, string iso)
        : base(name, iso)
    {
        Id = id;
    }
}

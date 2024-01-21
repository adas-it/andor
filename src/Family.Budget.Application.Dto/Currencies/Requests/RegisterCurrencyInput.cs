namespace Family.Budget.Application.Dto.Currencies.Requests;

public record RegisterCurrencyInput : BaseCurrency
{
    public RegisterCurrencyInput(string name,
        string iso) : base(name, iso)
    {
    }

    public RegisterCurrencyInput() : base()
    {
    }
}

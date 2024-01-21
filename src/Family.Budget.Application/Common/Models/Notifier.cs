namespace Family.Budget.Application.Models;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;

public record Notifier
{
    public Notifier()
    {
        Warnings = new List<ErrorModel>();
        Erros = new List<ErrorModel>();
    }

    public List<ErrorModel> Warnings { get; private set; }
    public List<ErrorModel> Erros { get; private set; }
}

namespace Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;

public class UserCreated
{
    public Guid UserId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid LanguageId { get; set; }
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public Guid Locale { get; set; }
    public string Email { get; set; } = "";
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }
}

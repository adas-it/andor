namespace Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;

public class RegistrationCompleted
{
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Locale { get; set; } = "";
    public string Email { get; set; } = "";
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }
}

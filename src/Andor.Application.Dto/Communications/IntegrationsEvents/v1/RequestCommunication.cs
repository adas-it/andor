namespace Andor.Application.Dto.Communications.IntegrationsEvents.v1;

public class RequestCommunication
{
    public Guid RuleId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public Guid? UserId { get; init; }
    public string? ContentLanguage { get; init; }
    public Dictionary<string, string>? Values { get; init; }
}

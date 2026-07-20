using Andor.Authorizations.Domain;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;

namespace Andor.Communications.Application.Commands;

public record SendNotificationCommand(RuleId Id, string RecipientEmail, string Subject,
    string TemplateTitle, Dictionary<string, string> Values, ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<RuleId>;

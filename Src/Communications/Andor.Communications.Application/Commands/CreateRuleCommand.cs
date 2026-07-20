using Andor.Authorizations.Domain;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Application.Commands;

public record CreateRuleTemplateInput(string Value, string ContentLanguage, string Title, Partner Partner);

public record CreateRuleCommand(RuleId Id, Name Name, Andor.Communications.Domain.ValueObjects.Type Type,
    List<CreateRuleTemplateInput> Templates, bool Force, ApplicationUser CurrentUser,
    CancellationToken CancellationToken) : ICommands<RuleId>;

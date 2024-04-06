using Andor.Application.Communications.Services.Manager;
using Andor.Domain.Entities.Communications.Repositories;
using MediatR;

namespace Andor.Application.Communications.Commands;

public record PublishCommunicationCommand(Guid RuleId,
    string? Email,
    string? Phone,
    Guid? UserId,
    string? ContentLanguage,
    Dictionary<string, string>? Values) : IRequest<Unit>;

public class PublishCommunicationCommandHandler(IQueriesRuleRepository _queriesRuleRepository,
    IPartnerManager _partnerManager)
    : IRequestHandler<PublishCommunicationCommand, Unit>
{
    public async Task<Unit> Handle(PublishCommunicationCommand request, CancellationToken cancellationToken)
    {
        var rule = await _queriesRuleRepository.GetByIdAsync(request.RuleId, cancellationToken);

        var language = request.ContentLanguage ?? "en";

        var template = rule.Templates.FirstOrDefault(x => x.ContentLanguage == language);

        var partnerHandler =
        _partnerManager.GetPartnerHandler(template.Partner);

        await partnerHandler.SendEmail(request.Email, template, request.Values, cancellationToken);

        return Unit.Value;
    }
}

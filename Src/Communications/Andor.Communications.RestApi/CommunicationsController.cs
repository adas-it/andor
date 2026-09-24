using System.Net.Mime;
using Andor.Authorizations.Domain;
using Andor.Communications.Application.Commands;
using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Requests;
using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Communications.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CommunicationsController(IRuleCommandsService commandsService,
    IMessageQueriesService messageQueriesService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost("rules")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<RuleOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRuleAsync([FromBody] CreateRuleInput input,
        CancellationToken cancellationToken)
    {
        var templates = input.Templates
            .Select(t => new CreateRuleTemplateInput(t.Value, t.ContentLanguage, t.Title,
                Enumeration<int>.GetByKey<Partner>(t.Partner)))
            .ToList();

        var command = new CreateRuleCommand(RuleId.New(),
            input.Name,
            Enumeration<int>.GetByKey<Andor.Communications.Domain.ValueObjects.Type>(input.Type),
            templates,
            input.Force,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.CreateRuleAsync(command);

        return Result<RuleOutput?>(output);
    }

    [HttpPost("notifications")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendNotificationAsync([FromBody] SendNotificationInput input,
        CancellationToken cancellationToken)
    {
        var command = new SendNotificationCommand(
            Id: RuleId.Load(input.RuleId),
            RecipientEmail: input.RecipientEmail,
            TemplateTitle: input.TemplateTitle,
            ContentLanguage: input.ContentLanguage,
            Values: input.Values,
            CurrentUser: currentUserService.GetCurrentUser(),
            CancellationToken: cancellationToken,
            RecipientId: input.RecipientId);

        var output = await commandsService.SendNotificationAsync(command);

        return Result<object?>(output);
    }

    // The push "inbox": the caller's own token decides whose messages come back, so there's no
    // separate scope check here beyond the class-level [Authorize] — this is an end-user route,
    // not a service-to-service one like the two above.
    [HttpGet("messages")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<IReadOnlyList<MessageOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessagesAsync(CancellationToken cancellationToken)
    {
        var output = await messageQueriesService.GetByRecipientAsync(
            currentUserService.GetCurrentUser().UserId, cancellationToken);

        return Result(output);
    }
}

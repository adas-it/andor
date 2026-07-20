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
        var command = new SendNotificationCommand(RuleId.Load(input.RuleId),
            input.RecipientEmail,
            input.Subject,
            input.TemplateTitle,
            input.Values,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.SendNotificationAsync(command);

        return Result<object?>(output);
    }
}

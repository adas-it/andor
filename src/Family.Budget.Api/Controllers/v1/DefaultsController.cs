namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Notifications.NotificationType;
using Family.Budget.Application.Models;
using Family.Budget.Application.MovementStatuses.Queries;
using Family.Budget.Application.MovementTypes.Queries;
using Family.Budget.Application.Notifications.Queries;
using Family.Budget.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class DefaultsController : BaseController
{
    private readonly IMediator mediator;

    private readonly ILogger<CategoriesController> _logger;
    public DefaultsController(ILogger<CategoriesController> logger, IMediator mediator, Notifier notifier) : base(notifier)
    {
        _logger = logger;

        this.mediator = mediator;
    }

    [HttpGet("financial-movement-status")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListFinancialMovementStatus(
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new ListFinancialMovementStatusQuery(), cancellationToken);

        return Result(output);
    }

    [HttpGet("financial-movement-type")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListFinancialMovementType(
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new ListFinancialMovementTypeQuery(), cancellationToken);

        return Result(output);
    }

    [HttpGet("notification-type")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<NotificationTypeOutput>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListNotificationType(
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new ListNotificationTypesQuery(), cancellationToken);

        return Result(output);
    }
}

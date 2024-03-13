using Asp.Versioning;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Notifications.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.Notifications.Queries;
using Family.Budget.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Family.Budget.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class NotificationsController : BaseController
{
    private readonly IMediator _mediator;

    private readonly ILogger<NotificationsController> _logger;
    public NotificationsController(ILogger<NotificationsController> logger,
        IMediator mediator, Notifier notifier) : base(notifier)
    {
        _logger = logger;

        _mediator = mediator;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<NotificationOutput>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllNotifications(CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new ListNotificationTypesQuery(),cancellationToken);
        return Result(output);
    }
}

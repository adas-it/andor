namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Users.Requests;
using Family.Budget.Application.Dto.Users.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.Users.Commands;
using Family.Budget.Api.Controllers.Base;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController : BaseController
{
    private readonly IMediator mediator;

    private readonly ILogger<UsersController> _logger;
    public UsersController(ILogger<UsersController> logger, IMediator mediator, Notifier notifier) : base(notifier)
    {
        _logger = logger;

        this.mediator = mediator;
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] RegisterUserInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = apiDto.Adapt<RegisterUserCommand>();

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }
}

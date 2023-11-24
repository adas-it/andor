namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Registrations.Registration;
using Family.Budget.Application.Dto.Registrations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.Registration.Commands;
using Family.Budget.Application.Registration.Queries;
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
public class RegistrationController : BaseController
{
    private readonly IMediator mediator;
    public RegistrationController(IMediator mediator, Notifier notifier) : base(notifier)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<RegistrationOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegistration(
        CancellationToken cancellationToken,
        [FromQuery] string email,
        [FromQuery] string code
    )
    {
        var input = new GetByEmailAndCodeQuery(email, code);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register(
        [FromBody] UserRegistrationInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var request = apiDto.Adapt<RegisterCommand>();

        await mediator.Send(request, cancellationToken);

        return Result<object>(null!); ;
    }

    [HttpPost("check")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CheckCodeEmail(
        [FromBody] RegistrationCheckEmail apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var request = apiDto.Adapt<CheckCodeEmailCommand>();

        await mediator.Send(request, cancellationToken);

        return Result<object>(null!); ;
    }

    [HttpPost("resubmit")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Resubmit(
        [FromBody] CoachRegistrationResubmitEmailInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var request = apiDto.Adapt<ResubmitCheckCodeCommand>();

        await mediator.Send(request, cancellationToken);

        return Result<object>(null!); ;
    }
}

using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Requests;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Application.Onboarding.Registrations.Commands;
using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Onboarding.Registrations.v1;


[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class RegistrationController(IMediator mediator) : BaseController
{
    /*
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
    */

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<RegistrationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Register(
        [FromBody] UserRegistrationInput input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var request = input.Adapt<RegisterCommand>();

        var output = await mediator.Send(request, cancellationToken);

        return Result(output);
    }
    
    [HttpPost("check")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<RegistrationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> CheckCodeEmail(
        [FromBody] RegistrationCheckEmail input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var request = input.Adapt<CheckCodeEmailCommand>();

        var output = await mediator.Send(request, cancellationToken);

        return Result(output);
    }

    /*

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
    */
}

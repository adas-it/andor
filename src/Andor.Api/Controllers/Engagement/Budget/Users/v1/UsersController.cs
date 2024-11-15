using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.User.Requests;
using Andor.Application.Dto.Engagement.Budget.User.Responses;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Engagement.Budget.Users.v1;


[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/users")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController(IMediator mediator) : BaseController
{
    private static UserOutput user = new UserOutput()
    {
        FirstName = "Batatinha",
        LastName = "Potatinho",
        Email = "batata@potato.com",
        Phone = "910641938",
        Birthday = "23/02/1993",
        Avatar = "https://conceito.de/wp-content/uploads/2012/11/utilizador.jpg",
        AvatarThumb = "https://conceito.de/wp-content/uploads/2012/11/utilizador.jpg"
    };

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> FullFill(
        CancellationToken cancellationToken
    )
    {
        return Result(ApplicationResult<UserOutput>.Success(Data: user));
    }

    [HttpPut]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromBody] UserRequest input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        user.LastName = input.LastName;
        user.FirstName = input.FirstName;
        user.Email = input.Email;

        return Results.Accepted();
    }

    [HttpPatch("change-password")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordRequest input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var a = ApplicationResult<UserOutput>.Failure(Errors: new List<ErrorModel>()
        {
            Application.Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation()});

        return Result(a);
    }

    [HttpPost("deactivate-account")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> DeactivateAccount(
        CancellationToken cancellationToken
    )
    {
        return Results.Accepted();
    }
}

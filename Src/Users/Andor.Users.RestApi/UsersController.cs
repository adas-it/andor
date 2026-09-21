using System.Net.Mime;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Application.Commands;
using Andor.Users.Application.Interfaces;
using Andor.Users.Contracts.Requests;
using Andor.Users.Contracts.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController(IUserQueriesService userQueries, IUserCommandsService userCommands) : BaseController
{
    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<UserPreferencesOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var output = await userQueries.GetByIdAsync(UserId.Load(id), cancellationToken);

        return Result(output);
    }

    // Called synchronously by Onboarding right after a signup is verified — a User (and, from
    // there, Identity credentials + a default Account) is part of the hard, must-happen chain
    // that ADR calls a "strong business rule", so it isn't left to best-effort choreography.
    [HttpPost]
    [MapToApiVersion("1.0")]
    [Authorize(Policy = "users.write")]
    [ProducesResponseType(typeof(DefaultResponse<UserPreferencesOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserInput input,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            UserId.Load(input.UserId),
            new Email(input.Email),
            input.FirstName,
            input.LastName,
            Guid.Empty,
            Guid.Empty,
            input.MarketingOptIn,
            input.TermsAndConditionsAccepted,
            input.PrivacyPolicyAccepted,
            input.PasswordHash,
            cancellationToken);

        var output = await userCommands.CreateUserAsync(command);

        return Result(output);
    }
}

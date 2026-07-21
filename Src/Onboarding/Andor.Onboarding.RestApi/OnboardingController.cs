using System.Net.Mime;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Contracts.Requests;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Onboarding.RestApi;

// Intentionally the one public controller in the system: this is the pre-login,
// landing-page signup flow, so it must work with no JWT at all.
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class OnboardingController(ISignupCommandsService commandsService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost("start")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartAsync([FromBody] StartSignupInput input,
        CancellationToken cancellationToken)
    {
        var output = await commandsService.StartSignupAsync(input.Name, input.Email,
            currentUserService.GetCurrentUser(), cancellationToken);

        return Result<object?>(output);
    }

    [HttpPost("verify")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifySignupInput input,
        CancellationToken cancellationToken)
    {
        var output = await commandsService.VerifySignupAsync(input.Email, input.Code, input.Password,
            currentUserService.GetCurrentUser(), cancellationToken);

        return Result<object?>(output);
    }
}

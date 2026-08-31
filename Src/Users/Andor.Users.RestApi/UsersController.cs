using System.Net.Mime;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Andor.Users.Application.Interfaces;
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
public class UsersController(IUserQueriesService userQueries) : BaseController
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
}

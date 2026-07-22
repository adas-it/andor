using System.Net.Mime;
using Andor.Assets.Application.Commands;
using Andor.Assets.Application.Interfaces;
using Andor.Assets.Contracts;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Assets.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AssetsController(IAreaCommandsService commandsService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AreaOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConfigurationAsync([FromBody] AreaInput input,
        CancellationToken cancellationToken)
    {
        var command = new CreateAreaCommand(AreaId.New(),
            input.Name,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.CreateAreaAsync(command);

        return Result<AreaOutput?>(output);
    }
}

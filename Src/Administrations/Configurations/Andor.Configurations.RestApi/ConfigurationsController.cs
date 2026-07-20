using System.Net.Mime;
using Andor.Authorizations.Domain;
using Andor.Configurations.Application.Commands;
using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Application.Queries;
using Andor.Configurations.Contracts.Requests;
using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Api;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Configurations.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ConfigurationsController(
    IConfigurationCommandsService configurationCommands,
    IConfigurationQueriesService configurationQueries,
    ICurrentUserService currentUserService,
    AuthorizationDomain authorizationDomain)
    : BaseController
{
    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConfigurationAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (await ForbidIfNotAuthorizedAsync(ConfigurationPermissions.ReadConfiguration) is { } forbidden)
        {
            return forbidden;
        }

        var config = await configurationQueries.GetByIdAsync(id, cancellationToken);

        return Result(config);
    }

    [HttpGet("Search")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaginatedListOutput<ConfigurationOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] PaginatedConfigurationListInput input,
        CancellationToken cancellationToken)
    {
        if (await ForbidIfNotAuthorizedAsync(ConfigurationPermissions.ReadConfiguration) is { } forbidden)
        {
            return forbidden;
        }

        var searchInput = new SearchConfigurationInput()
        {
            Page = input.Page,
            PerPage = input.PerPage,
            Search = input.Search,
            OrderBy = input.Sort,
            Order = (SearchOrder)(int)input.Dir,
            States = input.States.Select(x => ConfigurationState.GetByKey<ConfigurationState>(x)).ToArray()
        };

        searchInput.Normalize();

        var config = await configurationQueries.SearchAsync(
            searchInput,
            cancellationToken);

        return Result(config);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateConfigurationInput input,
        CancellationToken cancellationToken)
    {
        if (input == null)
        {
            return UnprocessableEntity();
        }

        if (await ForbidIfNotAuthorizedAsync(ConfigurationPermissions.CreateConfiguration) is { } forbidden)
        {
            return forbidden;
        }

        var command = new CreateConfigurationCommand(ConfigurationId.New(),
            input.Name, input.Value, input.Description, input.StartDate, input.ExpireDate, input.Force,
            currentUserService.GetCurrentUser(), cancellationToken);

        var output = await configurationCommands.CreateConfigurationAsync(command);

        return Result(output);
    }

    [HttpPut("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ChangeAsync(
        [FromBody] ChangeConfigurationInput input,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (input == null)
        {
            return UnprocessableEntity();
        }

        var command = new ChangeConfigurationCommand(id,
            input.Value, input.Description, input.StartDate, input.ExpireDate,
            currentUserService.GetCurrentUser(), cancellationToken);

        var output = await configurationCommands.ChangeConfigurationAsync(command);

        return Result(output);
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteConfigurationCommand(id, currentUserService.GetCurrentUser(), cancellationToken);

        var output = await configurationCommands.DeleteConfigurationAsync(command);

        _ = output.SetData((ConfigurationOutput)null!);

        return Result(output);
    }

    [HttpPatch("{id:guid}/deactivate")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeactivateAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateConfigurationCommand(id, currentUserService.GetCurrentUser(), cancellationToken);

        var output = await configurationCommands.DeactivateConfigurationAsync(command);

        _ = output.SetData((ConfigurationOutput)null!);

        return Result(output);
    }

    private async Task<IActionResult?> ForbidIfNotAuthorizedAsync(string permissionName)
    {
        if (await authorizationDomain.IsAuthorizedAsync(permissionName, null))
        {
            return null;
        }

        return Result<object?>(ApplicationResult<object?>.Failure([Errors.NotAuthorized()]));
    }
}

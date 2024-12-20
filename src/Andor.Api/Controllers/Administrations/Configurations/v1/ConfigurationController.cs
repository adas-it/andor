﻿using Andor.Application.Administrations.Configurations.Commands.ChangeConfigurations;
using Andor.Application.Administrations.Configurations.Commands.ModifyConfigurations;
using Andor.Application.Administrations.Configurations.Commands.RegisterConfigurations;
using Andor.Application.Administrations.Configurations.Commands.RemoveConfigurations;
using Andor.Application.Administrations.Configurations.Queries;
using Andor.Application.Dto.Administrations.Configurations.Requests;
using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Administrations.Configurations.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ConfigurationsController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Create(
        [FromBody] BaseConfiguration request,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            return Results.UnprocessableEntity();
        }

        var output = await _mediator.Send(new RegisterConfigurationCommand(request), cancellationToken);

        return Result(output);
    }

    [HttpPatch("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchConfiguration(
        [FromBody] JsonPatchDocument<BaseConfiguration> request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            return Results.UnprocessableEntity();
        }

        var output = CheckIdIfIdIsNull<ConfigurationOutput>(id);

        if (output.IsFailure)
        {
            return Result(output);
        }

        output = await _mediator.Send(new ModifyConfigurationCommand(id, request), cancellationToken);

        return Result(output);
    }

    [HttpPut("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromBody] BaseConfiguration request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = CheckIdIfIdIsNull<ConfigurationOutput>(id);

        if (output.IsFailure)
        {
            return Result(output);
        }

        output = await _mediator.Send(new ChangeConfigurationCommand(id, request), cancellationToken);

        return Result(output);
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = CheckIdIfIdIsNull<object>(id);

        if (output.IsFailure)
        {
            return Result(output);
        }

        output = await _mediator.Send(new RemoveConfigurationCommand(id), cancellationToken);

        return Result(output);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = CheckIdIfIdIsNull<ConfigurationOutput>(id);

        if (output.IsFailure)
        {
            return Result(output);
        }

        var config = await _mediator.Send(new GetConfigurationByIdQuery(id), cancellationToken);

        return Results.Ok(config);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListConfigurationsOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var output = await _mediator.Send(new ListConfigurationsQuery()
        {
            Page = page ?? 0,
            PerPage = perPage ?? 10,
            Search = search,
            Sort = sort,
            Dir = dir ?? SearchOrder.Asc
        }, cancellationToken);

        return Results.Ok(output);
    }
}

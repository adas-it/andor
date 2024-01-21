namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Administrations.Commands;
using Family.Budget.Application.Administrations.Queries;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.Request;
using Family.Budget.Application.Dto.Configurations.Requests;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Models;
using Family.Budget.Api.Controllers.Base;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ConfigurationsController : BaseController
{
    private readonly IMediator mediator;

    private readonly ILogger<ConfigurationsController> _logger;


    public ConfigurationsController(ILogger<ConfigurationsController> logger,
        IMediator mediator,
        Notifier notifier) : base(notifier)
    {
        _logger = logger;

        this.mediator = mediator;
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] RegisterConfigurationInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = apiDto.Adapt<RegisterConfigurationCommand>();

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }

    [HttpPatch("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PatchConfiguration(
        [FromBody] JsonPatchDocument<ModifyConfigurationInput> apiDto,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifyConfigurationInput, ModifyConfigurationCommand>();

        var entity = new PatchConfiguration(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }


    [HttpPut("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromBody] ModifyConfigurationInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var input = new ModifyConfigurationCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        await mediator.Send(new RemoveConfigurationCommand(id), cancellationToken);

        return Result<ConfigurationOutput>(null!);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var output = await mediator.Send(new GetByIdConfigurationQuery(id), cancellationToken);

        return Result(output);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListConfigurationsOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListConfigurationsQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var output = await mediator.Send(input, cancellationToken);
        return Result(output);
    }
}

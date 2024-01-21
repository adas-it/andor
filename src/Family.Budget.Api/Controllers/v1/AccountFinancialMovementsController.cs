namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.Request;
using Family.Budget.Application.Dto.FinancialMovements.Requests;
using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.FinancialMovements.Commands;
using Family.Budget.Application.FinancialMovements.Queries;
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
[Route("api/v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountFinancialMovementsController : BaseController
{
    private readonly IMediator mediator;

    private readonly ILogger<AccountFinancialMovementsController> _logger;
    public AccountFinancialMovementsController(ILogger<AccountFinancialMovementsController> logger,
        IMediator mediator,
        Notifier notifier) : base(notifier)
    {
        _logger = logger;

        this.mediator = mediator;
    }

    [HttpPost("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid accountId,
        [FromBody] RegisterFinancialMovementInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = apiDto.Adapt<RegisterFinancialMovementCommand>() with
        {
            AccountId = accountId
        };

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }

    [HttpPatch("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PatchFinancialMovement(
        [FromRoute] Guid accountId,
        [FromBody] JsonPatchDocument<ModifyFinancialMovementInput> apiDto,
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
            return Result<FinancialMovementOutput>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifyFinancialMovementInput, ModifyFinancialMovementCommand>();

        var entity = new PatchFinancialMovement(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }


    [HttpPut("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid accountId,
        [FromBody] ModifyFinancialMovementInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        var input = apiInput.Adapt<ModifyFinancialMovementCommand>();

        input.Id = id;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpDelete("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        await mediator.Send(new RemoveFinancialMovementCommand(id), cancellationToken);

        return Result<FinancialMovementOutput>(null!);
    }

    [HttpGet("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        var output = await mediator.Send(new GetByIdFinancialMovementQuery(id), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListFinancialMovementsOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null
    )
    {
        var input = new ListFinancialMovementsQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        if (year is not null) input.Year = year.Value;
        if (month is not null) input.Month = month.Value;

        input.AccountId = accountId;

        var output = await mediator.Send(input, cancellationToken);
        return Result(output);
    }
}

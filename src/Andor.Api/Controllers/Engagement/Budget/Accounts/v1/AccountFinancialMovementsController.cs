﻿using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Resquests;
using Asp.Versioning;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Engagement.Budget.Accounts.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountFinancialMovementsController : BaseController
{
    [HttpPost("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Create(
        [FromRoute] Guid accountId,
        [FromBody] RegisterFinancialMovementInput apiDto,
        CancellationToken cancellationToken
    )
    {
        /*
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
        */
        var result = ApplicationResult<FinancialMovementOutput>.Success();

        result.SetData(new FinancialMovementOutput());

        return Result(result);
    }

    [HttpPatch("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchFinancialMovement(
        [FromRoute] Guid accountId,
        [FromBody] JsonPatchDocument<ModifyFinancialMovementInput> apiDto,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
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
        */
        var result = ApplicationResult<FinancialMovementOutput>.Success();

        result.SetData(new FinancialMovementOutput());

        return Result(result);
    }


    [HttpPut("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromRoute] Guid accountId,
        [FromBody] ModifyFinancialMovementInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        var input = apiInput.Adapt<ModifyFinancialMovementCommand>();

        input.Id = id;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<FinancialMovementOutput>.Success();

        result.SetData(new FinancialMovementOutput());

        return Result(result);
    }

    [HttpDelete("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        await mediator.Send(new RemoveFinancialMovementCommand(id), cancellationToken);

        return Result<FinancialMovementOutput>(null!);
        */
        var result = ApplicationResult<object>.Success();

        return Result(result);
    }

    [HttpGet("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<FinancialMovementOutput>(null!);
        }

        var output = await mediator.Send(new GetByIdFinancialMovementQuery(id), cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<FinancialMovementOutput>.Success();

        result.SetData(new FinancialMovementOutput());

        return Result(result);
    }

    [HttpGet("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListFinancialMovementsOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
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
        /*
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
        */
        var result = ApplicationResult<ListFinancialMovementsOutput>.Success();

        result.SetData(new ListFinancialMovementsOutput(0, 0, 0, null));

        return Result(result);
    }
}

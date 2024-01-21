namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Categories.Commands;
using Family.Budget.Application.Categories.Queries;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Categories.Requests;
using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.Common.Request;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Models;
using Family.Budget.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Family.Budget.Domain.Entities.Accounts.ValueObject;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountCategoriesController : BaseController
{
    private readonly IMediator mediator;
    public AccountCategoriesController(IMediator mediator, Notifier notifier) : base(notifier)
    {
        this.mediator = mediator;
    }

    [HttpPost("{accountId:guid}/category")]
    public async Task<IActionResult> Create(
        [FromBody] RegisterCategoryInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = new RegisterCategoryCommand(apiDto);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }

    [HttpPatch("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PatchCategory(
        [FromBody] JsonPatchDocument<ModifyCategoryInput> apiDto,
        [FromRoute] Guid id,
        [FromRoute] AccountId accountId,
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
            return Result<object>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifyCategoryInput, ModifyCategoryCommand>();

        var entity = new PatchCategory(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }


    [HttpPut("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromBody] ModifyCategoryInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<object>(null!);
        }

        var input = new ModifyCategoryCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpDelete("{accountId:guid}/category/{id:guid}")]
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
            return Result<object>(null!);
        }

        await mediator.Send(new RemoveCategoryCommand(id), cancellationToken);

        return Result<CategoryOutput>(null!);
    }

    [HttpGet("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<object>(null!);
        }

        var output = await mediator.Send(new GetByIdCategoryQuery(id), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListCategoriesOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null,
        [FromQuery(Name = "type")] string? type = null
    )
    {
        var input = new ListCategoriesQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        if (type is not null) input.Type = type;

        var output = await mediator.Send(input, cancellationToken);
        return Result(output);
    }
}

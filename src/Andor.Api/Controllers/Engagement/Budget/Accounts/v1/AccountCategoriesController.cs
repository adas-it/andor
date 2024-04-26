using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Categories.Requests;
using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Application.Engagement.Budget.Accounts.Queries;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Engagement.Budget.Accounts.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountCategoriesController(IMediator mediator) : BaseController
{
    [HttpPost("{accountId:guid}/category")]
    public async Task<IResult> Create(
        [FromBody] CategoryInput apiDto,
        CancellationToken cancellationToken
    )
    {
        /*
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = new RegisterCategoryCommand(apiDto);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<CategoryOutput>.Success();

        result.SetData(new CategoryOutput());

        return Result(result);
    }

    [HttpPatch("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchCategory(
        [FromBody] JsonPatchDocument<CategoryInput> apiDto,
        [FromRoute] Guid id,
        [FromRoute] AccountId accountId,
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
            return Result<object>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifyCategoryInput, ModifyCategoryCommand>();

        var entity = new PatchCategory(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<CategoryOutput>.Success();

        result.SetData(new CategoryOutput());

        return Result(result);
    }


    [HttpPut("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromBody] CategoryInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<object>(null!);
        }

        var input = new ModifyCategoryCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<CategoryOutput>.Success();

        result.SetData(new CategoryOutput());

        return Result(result);
    }

    [HttpDelete("{accountId:guid}/category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<object>(null!);
        }

        await mediator.Send(new RemoveCategoryCommand(id), cancellationToken);

        return Result<CategoryOutput>(null!);
        */
        var result = ApplicationResult<object>.Success();

        return Result(result);
    }

    [HttpGet("{accountId:guid}/category/{categoryId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetByAccountIdAndCategoryIdQuery(
            accountId, categoryId), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListCategoriesOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null,
        [FromQuery(Name = "type")] int? type = null
    )
    {
        var input = new ListCategoriesQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        if (type is not null) input.Type = type.Value;

        var result = await mediator.Send(input, cancellationToken);

        return Result(result);
    }
}

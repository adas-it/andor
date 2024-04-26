using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Requests;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Application.Engagement.Budget.Accounts.Queries;
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
public class AccountSubCategoriesController(IMediator mediator) : BaseController
{
    [HttpPost("{accountId:guid}/sub-category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<SubCategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Create(
        [FromRoute] Guid accountId,
        [FromBody] RegisterSubCategoryInput apiDto,
        CancellationToken cancellationToken
    )
    {
        /*
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = new RegisterSubCategoryCommand(apiDto);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<SubCategoryOutput>.Success();

        result.SetData(new SubCategoryOutput());

        return Result(result);
    }

    [HttpPatch("{accountId:guid}/sub-category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<SubCategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchSubCategory(
        [FromRoute] Guid accountId,
        [FromBody] JsonPatchDocument<ModifySubCategoryInput> apiDto,
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
            return Result<SubCategoryOutput>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifySubCategoryInput, ModifySubCategoryCommand>();

        var entity = new PatchSubCategory(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<SubCategoryOutput>.Success();

        result.SetData(new SubCategoryOutput());

        return Result(result);
    }


    [HttpPut("{accountId:guid}/sub-category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<SubCategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromRoute] Guid accountId,
        [FromBody] ModifySubCategoryInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<SubCategoryOutput>(null!);
        }

        var input = new ModifySubCategoryCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<SubCategoryOutput>.Success();

        result.SetData(new SubCategoryOutput());

        return Result(result);
    }

    [HttpDelete("{accountId:guid}/sub-category/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<SubCategoryOutput>(null!);
        }

        await mediator.Send(new RemoveSubCategoryCommand(id), cancellationToken);

        return Result<SubCategoryOutput>(null!);
        */
        var result = ApplicationResult<object>.Success();


        return Result(result);
    }

    [HttpGet("{accountId:guid}/sub-category/{subCategoryId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<SubCategoryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid subCategoryId,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetByAccountIdAndSubCategoryIdQuery(accountId, subCategoryId)
            , cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/sub-category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListSubCategoriesOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null,
        [FromQuery(Name = "category")] Guid? category = null
    )
    {
        var input = new ListSubCategoriesQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        if (category is not null) input.CategoryId = category.Value;
        input.AccountId = accountId;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }
}

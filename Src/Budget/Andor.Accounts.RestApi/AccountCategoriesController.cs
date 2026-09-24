using System.Net.Mime;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Queries;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.Categories;
using Andor.Accounts.Contracts.Categories.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Requests;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Accounts.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountCategoriesController(IAccountCategoriesQueriesService service,
    IAccountCommandsService commandsService,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly IAccountCategoriesQueriesService _service = service;

    [HttpPost("{accountId:guid}/category")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid accountId,
        [FromBody] CreateCategoryInput input,
        CancellationToken cancellationToken
    )
    {
        MovementType type;
        try
        {
            type = MovementType.GetByKey<MovementType>(input.TypeId);
        }
        catch (InvalidOperationException)
        {
            return UnprocessableEntity();
        }

        var command = new CreateCustomCategoryCommand(
            AccountId.Load(accountId),
            new Name(input.Name),
            new Description(input.Description),
            type,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.CreateCustomCategoryAsync(command);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category/{categoryId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CategoryOutput?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken
    )
    {
        var output = await _service.GetByAccountIdAndCategoryIdAsync(accountId, categoryId, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListCategoriesOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromRoute(Name = "accountId")] Guid accountId,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null,
        [FromQuery(Name = "type")] int? type = null
    )
    {
        var input = new ListCategoriesQuery();
        if (page is not null)
            input.Page = page.Value;
        if (perPage is not null)
            input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search))
            input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort))
            input.OrderBy = sort;
        if (dir is not null)
            input.Order = (Andor.Foundation.Application.Queries.SearchOrder)dir.Value;
        if (type is not null)
            input.Type = type.Value;

        input.AccountId = accountId;

        input.Normalize();

        var output = await _service.GetByAccountIdAndCategoryAsync(input, cancellationToken);

        return Result(output);
    }
}

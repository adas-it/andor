using System.Net.Mime;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Queries;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.SubCategories;
using Andor.Accounts.Contracts.SubCategories.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
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
public class AccountSubCategoriesController(IAccountSubCategoriesQueriesService service,
    IAccountCommandsService commandsService,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly IAccountSubCategoriesQueriesService _service = service;

    [HttpPost("{accountId:guid}/sub-category")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid accountId,
        [FromBody] CreateSubCategoryInput input,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateCustomSubCategoryCommand(
            AccountId.Load(accountId),
            new Name(input.Name),
            new Description(input.Description),
            CategoryId.Load(input.CategoryId),
            input.DefaultPaymentMethodId.HasValue ? PaymentMethodId.Load(input.DefaultPaymentMethodId.Value) : null,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.CreateCustomSubCategoryAsync(command);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/sub-category/{subCategoryId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<SubCategoryOutput?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid subCategoryId,
        CancellationToken cancellationToken
    )
    {
        var output = await _service.GetByAccountIdAndSubCategoryIdAsync(accountId, subCategoryId, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/sub-category")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListSubCategoriesOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
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
        if (category is not null)
            input.CategoryId = (CategoryId)category.Value;
        input.AccountId = accountId;

        var output = await _service.GetByAccountIdAndSubCategoryAsync(input, cancellationToken);

        return Result(output);
    }
}

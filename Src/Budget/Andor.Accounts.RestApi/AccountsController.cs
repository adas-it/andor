using System.Net.Mime;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Accounts.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountsController(IAccountCommandsService commandsService,
    IAccountQueriesService accountQueriesService,
    ICurrentUserService currentUserService) : BaseController
{

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListAccountOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] Andor.Foundation.Contracts.Requests.SearchOrder? dir = null
    )
    {
        var searchInput = new SearchInput(page, perPage, search, sort, (SearchOrder?)dir);

        searchInput.Normalize();

        var output = await accountQueriesService.GetListAsync(searchInput, cancellationToken);

        return Result(output);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var output = await accountQueriesService.GetByIdAsync(AccountId.Load(id), cancellationToken);

        return Result<AccountOutput?>(output);
    }

    [HttpGet("{accountId:guid}/cash-flow")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CashFlowOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashFlow(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "month")] int? month = null
    )
    {
        var output = await accountQueriesService.GetCashFlowAsync(
            AccountId.Load(accountId), Month.Load(month ?? DateTime.UtcNow.Month), new Year(year ?? DateTime.UtcNow.Year), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-summary")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<FinancialSummariesOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FinancialSummary(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "month")] int? month = null
    )
    {
        var output = await accountQueriesService.GetFinancialSummaryAsync(
            AccountId.Load(accountId), Month.Load(month ?? DateTime.UtcNow.Month), new Year(year ?? DateTime.UtcNow.Year), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category-summary")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<CategorySummariesOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CategorySummary(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "month")] int? month = null
    )
    {
        var output = await accountQueriesService.GetCategorySummaryAsync(
            AccountId.Load(accountId), Month.Load(month ?? DateTime.UtcNow.Month), new Year(year ?? DateTime.UtcNow.Year), cancellationToken);

        return Result(output);
    }
}

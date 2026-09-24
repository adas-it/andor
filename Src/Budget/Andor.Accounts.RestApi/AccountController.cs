using System.Net.Mime;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts.Accounts.Requests;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
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
public class AccountController(IAccountCommandsService commandsService,
    IAccountQueriesService accountQueriesService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] Andor.Accounts.Contracts.AccountInput input,
        CancellationToken cancellationToken
    )
    {
        var currentUser = currentUserService.GetCurrentUser();

        var command = new CreateAccountCommand(
            AccountId.New(),
            new Name(input.Name),
            new Description(input.Name),
            CurrencyId.Load(input.CurrencyId),
            currentUser,
            cancellationToken);

        var output = await commandsService.CreateAccountAsync(command);

        return Result(output);
    }

    [HttpPost("{accountId:guid}/seed")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CashFlowOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedDefault(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId
    )
    {
        var currentUser = currentUserService.GetCurrentUser();

        var command = new SeedAccountDefaultsCommand(
            AccountId.Load(accountId),
            currentUser,
            cancellationToken);

        var output = await commandsService.SeedAccountDefaultsAsync(command);

        return Result(output);
    }

    [HttpPut("{accountId:guid}/update-details")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateDetails(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromBody] AccountDetails input
    )
    {
        var currentUser = currentUserService.GetCurrentUser();

        var command = new UpdateAccountDetailsCommand(
            AccountId.Load(accountId),
            new Name(input.Name),
            new Description(input.description),
            CurrencyId.Load(input.CurrencyId),
            currentUser,
            cancellationToken);

        var output = await commandsService.UpdateAccountDetailsAsync(command);

        return Result(output);
    }

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

    [HttpGet("currency")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<CurrencyOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrency(
        CancellationToken cancellationToken
    )
    {
        var list = Andor.Shared.Lookups.Currency.GetAll().Select(c => new CurrencyOutput(c.Id.ToString(), c.Name, c.Symbol)).ToList();

        var output = ApplicationResult<List<CurrencyOutput>>.Success(Data: list);

        return Result(output);
    }

    [HttpGet("permission-types")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<PermissionTypeOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissionTypes(
        CancellationToken cancellationToken
    )
    {
        var list = PermissionType.GetAll<PermissionType>().Select(c => new PermissionTypeOutput(c.Key, c.Name)).ToList();

        var output = ApplicationResult<List<PermissionTypeOutput>>.Success(Data: list);

        return Result(output);
    }
}

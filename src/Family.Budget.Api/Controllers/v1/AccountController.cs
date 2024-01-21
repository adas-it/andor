namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Accounts.Commands;
using Family.Budget.Application.Accounts.Queries;
using Family.Budget.Application.Dto.Accounts.Requests;
using Family.Budget.Application.Dto.Accounts.Responses;
using Family.Budget.Application.Dto.CashFlows.Responses;
using Family.Budget.Application.Dto.Common.Request;
using Family.Budget.Application.Dto.FinancialSummaries.Responses;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.FinancialSummaries.Queries;
using Family.Budget.Application.Models;
using Family.Budget.Application.MonthlyCashFlow.Queries;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountController : BaseController
{
    private readonly IMediator mediator;
    public AccountController(IMediator mediator, Notifier notifier) : base(notifier)
    {
        this.mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<AccountOutput>(null!);
        }

        var output = await mediator.Send(new GetByIdAccountQuery() { AccountId = id}, cancellationToken);

        return Result(output);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListAccountOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListAccountsQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var output = await mediator.Send(input, cancellationToken);
        return Result(output);
    }

    [HttpGet("{accountId:guid}/cash-flow")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CashFlowOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashFlow(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null
    )
    {
        GetCashFlowByMonthQuery input = new();
        input.AccountId = accountId;
        input.Year = year ??  DateTime.UtcNow.Year;
        input.Month = month ?? DateTime.UtcNow.Month;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-summary")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<FinancialSummariesOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FinancialSummary(
        CancellationToken cancellationToken,
        [FromRoute] Guid? accountId,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "month")] int? month = null
    )
    {
        var input = new GetFinancialSummariesByMonthQuery();
        if (accountId is not null) input.AccountId = accountId.Value;
        if (year is not null) input.Year = year.Value;
        if (month is not null) input.Month = month.Value;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/category-summary")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<CategorySummariesOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CategorySummary(
        CancellationToken cancellationToken,
        [FromRoute] Guid? accountId,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "month")] int? month = null
    )
    {
        var input = new GetCategorySummariesByMonthQuery();
        if (accountId is not null) input.AccountId = accountId.Value;
        if (year is not null) input.Year = year.Value;
        if (month is not null) input.Month = month.Value;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpPost("{accountId:guid}/share")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ShareAccount(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromBody] ShareInput input
    )
    {
        var command = new ShareCommand()
        {
            AccountId = accountId,
            Email = input.Email,
        };

        var output = await mediator.Send(command, cancellationToken);

        return Result("");
    }
}

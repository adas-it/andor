using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Requests;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Application.Engagement.Budget.Accounts.Queries;
using Andor.Application.Engagement.Budget.MonthlyCash.Queries;
using Andor.Application.Onboarding.Registrations.Commands;
using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Onboarding.Registrations.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountController(IMediator mediator) : BaseController
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListAccountOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAccounts(
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

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetByIdAccountQuery() { AccountId = id }, cancellationToken);

        return Result(output);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<RegistrationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Register(
        [FromBody] UserRegistrationInput input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var request = input.Adapt<RegisterCommand>();

        var output = await mediator.Send(request, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/cash-flow")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<CashFlowOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> GetCashFlow(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null
    )
    {
        GetCashFlowByMonthQuery input = new();
        input.AccountId = accountId;
        input.Year = year ?? DateTime.UtcNow.Year;
        input.Month = month ?? DateTime.UtcNow.Month;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-summary")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<FinancialSummariesOutput>>), StatusCodes.Status200OK)]
    public async Task<IResult> FinancialSummary(
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
    public async Task<IResult> CategorySummary(
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
}

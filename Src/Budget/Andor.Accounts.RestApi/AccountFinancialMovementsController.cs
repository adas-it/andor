using System.Net.Mime;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Queries;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Contracts.FinancialMovements.Resquests;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Requests;
using Andor.Foundation.Contracts.Results;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Accounts.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountFinancialMovementsController(IAccountFinancialMovementsQueriesService service,
    IAccountCommandsService accountService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterFinancialMovement(
        [FromRoute] Guid accountId,
        [FromBody] RegisterFinancialMovementInput input,
        CancellationToken cancellationToken
    )
    {
        var command = new AddFinancialMovementCommand(
            Id: accountId,
            Date: input.Date,
            Description: input.Description,
            SubCategoryId: input.SubCategoryId,
            PaymentMethodId: input.PaymentMethodId,
            Value: input.Value,
            Status: MovementStatus.GetByKey<MovementStatus>(input.StatusId),
            CurrentUser: currentUserService.GetCurrentUser(),
            CancellationToken: cancellationToken
        );

        var output = await accountService.AddFinancialMovementAsync(command);

        return Result(output);
    }

    [HttpPut("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid accountId,
        [FromBody] ModifyFinancialMovementInput input,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var command = new EditFinancialMovementCommand(
            Id: accountId,
            FinancialMovementId: id,
            Date: input.Date,
            Description: input.Description,
            SubCategoryId: input.SubCategoryId,
            PaymentMethodId: input.PaymentMethodId,
            Value: input.Value,
            Status: MovementStatus.GetByKey<MovementStatus>(input.StatusId),
            CurrentUser: currentUserService.GetCurrentUser(),
            CancellationToken: cancellationToken
        );

        var output = await accountService.EditFinancialMovementAsync(command);

        return Result(output);
    }

    [HttpDelete("{accountId:guid}/financial-movement/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteFinancialMovementCommand(
            Id: accountId,
            FinancialMovementId: id,
            CurrentUser: currentUserService.GetCurrentUser(),
            CancellationToken: cancellationToken
        );

        var output = await accountService.DeleteFinancialMovementAsync(command);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-movement/{financialMovementId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<FinancialMovementOutput?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid financialMovementId,
        CancellationToken cancellationToken
    )
    {
        var output = await service.GetByAccountIdAndFinancialMovementIdAsync(accountId, financialMovementId, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/financial-movement")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListFinancialMovementsOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
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
        var input = new ListFinancialMovementsQuery();
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

        input.Year = year ?? DateTime.UtcNow.Year;
        input.Month = month ?? DateTime.UtcNow.Month;
        input.AccountId = accountId;

        var output = await service.GetByAccountIdAndFinancialMovementAsync(input, cancellationToken);

        return Result(output);
    }
}

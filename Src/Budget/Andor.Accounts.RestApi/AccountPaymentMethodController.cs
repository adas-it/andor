using System.Net.Mime;
using Andor.Accounts.Application.Queries;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.PaymentMethods.Responses;
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
public class AccountPaymentMethodController(IAccountPaymentMethodQueriesService service) : BaseController
{
    private readonly IAccountPaymentMethodQueriesService _service = service;

    [HttpGet("{accountId:guid}/payment-method/{paymentMethodId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid paymentMethodId,
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var output = await _service.GetByAccountIdAndPaymentMethodIdAsync(accountId, paymentMethodId, cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/payment-method")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListPaymentMethodsOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromRoute] Guid accountId,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery(Name = "type")] int? type = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListPaymentMethodsQuery();
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

        var output = await _service.GetByAccountIdAndPaymentMethodAsync(input, cancellationToken);

        return Result(output);
    }
}

using System.Net.Mime;
using Andor.Accounts.Application.Commands;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts;
using Andor.Accounts.Contracts.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Accounts.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountsController(IAccountCommandsService commandsService,
    IAccountQueriesService accountQueriesService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAsync([FromBody] AccountInput input,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(AccountId.New(),
            input.Name,
            "Conta criada via API",
            CurrencyId.Load(input.CurrencyId),
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.CreateAccountAsync(command);

        return Result<AccountOutput?>(output);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var output = await commandsService.GetByIdAsync(AccountId.Load(id), cancellationToken);

        return Result<AccountOutput?>(output);
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
}

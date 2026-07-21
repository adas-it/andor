using System.Net.Mime;
using Andor.Accounts.Application.Commands;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
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
}

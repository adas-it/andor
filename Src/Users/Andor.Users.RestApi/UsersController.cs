using System.Net.Mime;
using Andor.Foundation.Api;
using Andor.Foundation.Contracts.Results;
using Andor.Users.Application.Interfaces;
using Andor.Users.Contracts.Responses;
using Andor.Users.Domain.Users.ValueObjects;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Users.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UsersController(IUserQueriesService userQueries, IUserCommandsService userCommands) : BaseController
{
    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<UserPreferencesOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var output = await userQueries.GetByIdAsync(UserId.Load(id), cancellationToken);

        return Result(output);
    }


    [HttpGet("{id:guid}/public-data")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<UserPublicData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicDataByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var output = await userQueries.GetPublicDataByIdAsync(UserId.Load(id), cancellationToken);

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

    [HttpGet("language")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<List<LanguageOutput>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLanguage(
        CancellationToken cancellationToken
    )
    {
        var list = Andor.Shared.Lookups.Language.GetAll().Select(c => new LanguageOutput(c.Id.ToString(), c.Name, c.ISO)).ToList();

        var output = ApplicationResult<List<LanguageOutput>>.Success(Data: list);

        return Result(output);
    }
}

using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Accounts.RestApi;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class InvitesController
{
    /*
    [HttpGet("invites")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListInviteOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAccounts(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListInvitesQuery();
        if (page is not null)
            input.Page = page.Value;
        if (perPage is not null)
            input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search))
            input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort))
            input.Sort = sort;
        if (dir is not null)
            input.Dir = dir.Value;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }
    [HttpPost("{accountid:guid}/share")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<InviteOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Share(
        [FromRoute] Guid accountId,
        [FromBody] InviteInput input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var request = new CreateInviteCommand()
        {
            AccountId = accountId,
            Email = input.Email
        };

        request.AccountId = accountId;

        var output = await mediator.Send(request, cancellationToken);

        return Result(output);
    }

    [HttpPost("{inviteId:guid}/answer")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Answer(
        [FromRoute] Guid inviteId,
        [FromBody] AnswerInput input,
        CancellationToken cancellationToken
    )
    {
        if (input == null)
        {
            return Results.UnprocessableEntity();
        }

        var request = new AnswerInviteCommand()
        {
            InviteId = inviteId,
            IsAccepeted = input.IsAccepted
        };

        var output = await mediator.Send(request, cancellationToken);

        return Result(output);
    }*/
}

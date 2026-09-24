using System.Net.Mime;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.Invites;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Invites.ValueObjects;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Accounts.Domain.Users.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Api;
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
[Route("v{version:apiVersion}/account/{accountId:guid}/invites")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class InvitesController(IAccountCommandsService commandsService,
    IAccountQueriesService queriesService,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateInvite(
        [FromRoute] Guid accountId,
        [FromBody] InviteInput input,
        CancellationToken cancellationToken
    )
    {
        PermissionType permission;
        try
        {
            permission = PermissionType.GetByKey<PermissionType>(input.PermissionKey);
        }
        catch (InvalidOperationException)
        {
            return UnprocessableEntity();
        }

        var currentUser = currentUserService.GetCurrentUser();

        ApplicationResult<AccountOutput?> output;

        if (!string.IsNullOrWhiteSpace(input.Email))
        {
            var command = new InviteMemberByEmailCommand(
                AccountId.Load(accountId),
                new Email(input.Email),
                permission,
                currentUser,
                cancellationToken);

            output = await commandsService.InviteMemberByEmailAsync(command);
        }
        else if (input.UserId.HasValue)
        {
            var command = new InviteMemberByUserCommand(
                AccountId.Load(accountId),
                UserId.Load(input.UserId.Value),
                permission,
                currentUser,
                cancellationToken);

            output = await commandsService.InviteMemberByUserAsync(command);
        }
        else
        {
            return UnprocessableEntity();
        }

        return Result(output);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<Andor.Accounts.Contracts.Invites.Responses.ListInviteOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListInvites(
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var output = await queriesService.GetInvitesAsync(AccountId.Load(accountId), cancellationToken);

        return Result(output);
    }

    [HttpPost("{inviteId:guid}/answer")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<AccountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnswerInvite(
        [FromRoute] Guid accountId,
        [FromRoute] Guid inviteId,
        [FromBody] AnswerInviteInput input,
        CancellationToken cancellationToken
    )
    {
        var command = new AnswerInviteCommand(
            AccountId.Load(accountId),
            InviteId.Load(inviteId),
            input.Accept,
            currentUserService.GetCurrentUser(),
            cancellationToken);

        var output = await commandsService.AnswerInviteAsync(command);

        return Result(output);
    }
}

using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Communications.Requests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Communications.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/communication")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CommunicationController(IMediator mediator) : BaseController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public Task<IResult> OptInOutAsync([FromBody] OptInOut request, CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            return Task.FromResult(Results.UnprocessableEntity());
        }

        return Task.FromResult(Results.Accepted());
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListNotificationsOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public Task<IResult> GetNotificationsAsync(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null)
    {
        var list = new List<NotificationsOutput>()
        {
            new NotificationsOutput(Guid.NewGuid(),"1","Prevista para o Movimento.","Olha aqui que mensagem linda! mas porque sera que esta comendo o texto?",
                "2 min ago",
                false,
                0)
        };

        var output = new ListNotificationsOutput(1, 10, 100, list);

        return Task.FromResult(Result(ApplicationResult<ListNotificationsOutput>.Success(Data: output)));
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = CheckIdIfIdIsNull<object>(id);

        return Task.FromResult(Results.NoContent());
    }
}

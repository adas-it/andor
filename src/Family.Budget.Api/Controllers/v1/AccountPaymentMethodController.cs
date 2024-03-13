namespace Family.Budget.Api.Controllers.v1;

using Asp.Versioning;
using Family.Budget.Application.Common.Extensions;
using Family.Budget.Application.Dto.Common.Request;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.PaymentMethods.Requests;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Application.PaymentMethod.Commands;
using Family.Budget.Application.PaymentMethod.Queries;
using Family.Budget.Api.Controllers.Base;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountPaymentMethodController : BaseController
{
    private readonly IMediator mediator;

    private readonly ILogger<PaymentMethodController> _logger;
    public AccountPaymentMethodController(ILogger<PaymentMethodController> logger, IMediator mediator, Notifier notifier) : base(notifier)
    {
        _logger = logger;

        this.mediator = mediator;
    }

    [HttpPost("{accountId:guid}/payment-method")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] RegisterPaymentMethodInput apiDto,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = apiDto.Adapt<RegisterPaymentMethodCommand>();

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }

    [HttpPatch("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PatchPaymentMethod(
        [FromBody] JsonPatchDocument<ModifyPaymentMethodInput> apiDto,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        var input = apiDto.MapPatchInputToPatchCommand<ModifyPaymentMethodInput, ModifyPaymentMethodCommand>();

        var entity = new PatchPaymentMethod(id, input);

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
    }


    [HttpPut("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromBody] ModifyPaymentMethodInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        var input = new ModifyPaymentMethodCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }

    [HttpDelete("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        await mediator.Send(new RemovePaymentMethodCommand(id), cancellationToken);

        return Result<PaymentMethodOutput>(null!);
    }

    [HttpGet("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        var output = await mediator.Send(new GetByIdPaymentMethodQuery(id), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/payment-method")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListPaymentMethodsOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] Guid? accountId = null,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery(Name = "type")] string? type = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListPaymentMethodsQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        if (type is not null) input.Type = type;

        var output = await mediator.Send(input, cancellationToken);
        return Result(output);
    }
}

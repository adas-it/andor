using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Requests;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Application.Engagement.Budget.Accounts.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Andor.Api.Controllers.Engagement.Budget.Accounts.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/account")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AccountPaymentMethodController(IMediator mediator) : BaseController
{
    [HttpPost("{accountId:guid}/payment-method")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Create(
        [FromBody] RegisterPaymentMethodInput apiDto,
        CancellationToken cancellationToken
    )
    {
        /*
        if (apiDto == null)
        {
            return UnprocessableEntity(new DefaultResponse<object>());
        }

        var entity = apiDto.Adapt<RegisterPaymentMethodCommand>();

        var output = await mediator.Send(entity, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<PaymentMethodOutput>.Success();

        result.SetData(new PaymentMethodOutput());

        return Result(result);
    }

    [HttpPatch("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchPaymentMethod(
        [FromBody] JsonPatchDocument<ModifyPaymentMethodInput> apiDto,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
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
        */
        var result = ApplicationResult<PaymentMethodOutput>.Success();

        result.SetData(new PaymentMethodOutput());

        return Result(result);
    }


    [HttpPut("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromBody] ModifyPaymentMethodInput apiInput,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        var input = new ModifyPaymentMethodCommand(id, apiInput);

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
        */
        var result = ApplicationResult<PaymentMethodOutput>.Success();

        result.SetData(new PaymentMethodOutput());

        return Result(result);
    }

    [HttpDelete("{accountId:guid}/payment-method/{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
    [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        /*
        CheckIdIfIdIsNull(id);

        if (notifier.Erros.Any())
        {
            return Result<PaymentMethodOutput>(null!);
        }

        await mediator.Send(new RemovePaymentMethodCommand(id), cancellationToken);

        return Result<PaymentMethodOutput>(null!);
        */
        var result = ApplicationResult<object>.Success();

        return Result(result);
    }

    [HttpGet("{accountId:guid}/payment-method/{paymentMethodId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<PaymentMethodOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid paymentMethodId,
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var output = await mediator.Send(new GetByIdPaymentMethodQuery(accountId, paymentMethodId), cancellationToken);

        return Result(output);
    }

    [HttpGet("{accountId:guid}/payment-method")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListPaymentMethodsOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
        CancellationToken cancellationToken,
        [FromRoute] Guid? accountId = null,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery(Name = "type")] int? type = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListPaymentMethodsQuery();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        if (type is not null) input.Type = type.Value;

        input.AccountId = accountId.Value;

        var output = await mediator.Send(input, cancellationToken);

        return Result(output);
    }
}

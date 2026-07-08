using System.Diagnostics;
using Andor.Foundation.Contracts.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Andor.Foundation.Api;

public class BaseController() : ControllerBase
{
    protected IActionResult Result<T>(ApplicationResult<T> response) where T : class?
        => Result<T>(response, false);

    protected IActionResult Result<T>(ApplicationResult<T>? response, bool treatEmptyAsNotFound)
    where T : class?
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;

        if (response is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new DefaultResponse<object>
                {
                    TraceId = traceId
                });
        }

        var responseDto = new DefaultResponse<T>
        {
            Data = response.Data,
            TraceId = traceId,
        };

        responseDto.Infos.AddRange(response.Infos);
        responseDto.Warnings.AddRange(response.Warnings);
        responseDto.Errors.AddRange(response.Errors);

        if (responseDto.Errors.Count > 0)
        {
            return BadRequest(responseDto);
        }

        if (responseDto.Data is not null)
        {
            return Ok(responseDto);
        }

        if (treatEmptyAsNotFound)
        {
            return NotFound(new DefaultResponse<object> { TraceId = traceId });
        }

        return NoContent();
    }

    protected ApplicationResult<T> IdIsNullOrEmpty<T>(Guid? id) where T : class
    {
        var response = ApplicationResult<T>.Success();

        if (id == null || id == Guid.Empty)
        {
            var err = Errors.Validation();

            err.ChangeInnerMessage("Id cannot be null");

            response.AddError(err);
        }

        return response;
    }
}

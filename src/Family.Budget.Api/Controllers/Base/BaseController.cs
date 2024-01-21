using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Family.Budget.Api.Controllers.Base;

public class BaseController : ControllerBase
{
    protected Notifier notifier { get; init; }

    private static readonly string SourceName = "PersonalTrainer";

    protected ActivitySource MyActivitySource = new(SourceName);

    public BaseController(Notifier notifier)
    {
        this.notifier = notifier;
    }

    protected IActionResult Result<T>(T? model) where T : class
    {
        if (model is null && !notifier.Warnings.Any() && !notifier.Erros.Any())
        {
            return NoContent();
        }

        DefaultResponse<T> responseDto = new(model!);

        if (notifier.Warnings.Any())
        {
            responseDto.Warnings.AddRange(notifier.Warnings);
        }

        if (notifier.Erros.Any())
        {
            responseDto.Errors.AddRange(notifier.Erros);
        
            return BadRequest(responseDto);
        }

        return Ok(responseDto);
    }

    protected void CheckIdIfIdIsNull(Guid id)
    {
        if (id == Guid.Empty)
        {
            var err = Errors.Validation();

            err.ChangeInnerMessage("Id cannot be null");

            this.notifier.Erros.Add(err);

        }
    }
}
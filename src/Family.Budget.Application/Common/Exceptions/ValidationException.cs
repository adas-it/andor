namespace Family.Budget.Application._Common.Exceptions;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

public sealed class ValidationException : Exception, IApiException
{
    public List<ErrorModel> Errors { get; }

    public HttpStatusCode Status => HttpStatusCode.BadRequest;

    [JsonConstructor]
    public ValidationException(List<ErrorModel> errors)
        : base(string.Join(" | ", errors.Select(e => e.Message)))
    {
        Errors = errors;
    }

    public ValidationException(ErrorModel error)
        : base(error.Message)
    {
        Errors = new List<ErrorModel> { error };
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new List<ErrorModel> { new(Dto.Models.Errors.Errors.Validation().Code, message) };
    }

    public static ValidationException Build(IEnumerable<ValidationResult> validationResults)
    {
        var applicationErrors = validationResults
            .Where(validationResult => !validationResult.IsValid)
        .Select(error => new ErrorModel(
            Dto.Models.Errors.Errors.Validation().Code,
            string.Join(Environment.NewLine, error.Errors.Select(er => er.ErrorMessage)))
            ).ToList();

        return new ValidationException(applicationErrors);
    }
}
namespace Family.Budget.Application.Common.Exceptions;

using Family.Budget.Application._Common.Exceptions;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
public class GenericException : Exception, IApiException
{
    public List<ErrorModel> Errors { get; }
    public HttpStatusCode Status { get; }

    [JsonConstructor]
    public GenericException(List<ErrorModel> errors)
        : base(string.Join(" | ", errors.Select(e => e.Message)))
    {
        Status = HttpStatusCode.InternalServerError;
        Errors = errors;
    }

    public GenericException(ErrorModel error)
        : this(new List<ErrorModel> { error })
    {
    }
}

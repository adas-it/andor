namespace Family.Budget.Application._Common.Exceptions;

using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using System.Collections.Generic;
using System.Net;

public interface IApiException
{
    List<ErrorModel> Errors { get; }
    HttpStatusCode Status { get; }
}

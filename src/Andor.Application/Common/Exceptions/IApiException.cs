using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using System.Net;

namespace Andor.Application.Common.Exceptions;

public interface IApiException
{
    List<ErrorModel> Errors { get; }
    HttpStatusCode Status { get; }
}

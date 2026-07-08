
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Andor.Foundation.Contracts.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Andor.Foundation.ServerServices.Middlewares;

public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error | {request_path}", context.Request.Path.Value);
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        jsonOptions.Converters.Add(new ErrorCodeConverter());

        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        string serialized;
        int statusCode;

        switch (exception)
        {

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                serialized = JsonSerializer.Serialize(
                    new DefaultResponse<object>(null!, Errors.Generic().ChangeInnerMessage(exception.Message), traceId),
                    jsonOptions);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(serialized);
    }
}

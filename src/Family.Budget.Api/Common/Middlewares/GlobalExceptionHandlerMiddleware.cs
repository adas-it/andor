using Family.Budget.Application._Common.Exceptions;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Models.Errors;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Family.Budget.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro inesperado | {context.Request.Path.Value}");
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        string serialized;
        int statusCode;

        switch (exception)
        {
            case IApiException apiException:
                statusCode = (int)apiException.Status;
                serialized = JsonSerializer.Serialize(new DefaultResponse<object>(null!, apiException.Errors, traceId),
                    jsonOptions);
                break;

            case DbUpdateException:
            case DBConcurrencyException:
                statusCode = (int)HttpStatusCode.InternalServerError;
                serialized = JsonSerializer.Serialize(
                    new DefaultResponse<object>(null!, Errors.GenericDataBaseError().ChangeInnerMessage(exception.Message), traceId),
                    jsonOptions);
                break;

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
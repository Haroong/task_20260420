using System.Net;
using System.Text.Json;
using task_20260420.Common.Exceptions;
using task_20260420.Common.Models;

namespace task_20260420.Common.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code, message) = exception is AppException appEx
            ? (appEx.StatusCode, appEx.Code, appEx.Message)
            : (
                HttpStatusCode.InternalServerError,
                ResponseCode.ServerError,
                "서버 내부 오류가 발생했습니다.");

        if (statusCode >= HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "요청 처리 중 서버 오류 발생: {Message}", exception.Message);
        else
            _logger.LogWarning(exception, "요청 처리 중 클라이언트 오류 발생: {StatusCode} {Message}", (int)statusCode, exception.Message);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = BaseResponse<object?>.OnFailure(code, message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

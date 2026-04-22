using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using task_20260420.Common.Exceptions;
using task_20260420.Common.Middleware;

namespace task_20260420.Tests.Unit.Common;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_AppException_ReturnsMappedStatusAndCode()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidFormatException("잘못된 형식입니다."),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var body = await ReadResponseBody(context);
        body.GetProperty("code").GetString().Should().Be("FORMAT400");
        body.GetProperty("message").GetString().Should().Be("잘못된 형식입니다.");
    }

    [Fact]
    public async Task InvokeAsync_UnknownException_ReturnsInternalServerError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new Exception("boom"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        var body = await ReadResponseBody(context);
        body.GetProperty("code").GetString().Should().Be("SERVER500");
    }

    private static async Task<JsonElement> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<JsonElement>(json);
    }
}

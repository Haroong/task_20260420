using System.Diagnostics;
using MediatR;
using task_20260420.Common.Exceptions;

namespace task_20260420.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("[START] {RequestName} {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("[END] {RequestName} completed in {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            if (ex is AppException)
                _logger.LogWarning(ex, "[FAIL] {RequestName} failed after {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);
            else
                _logger.LogError(ex, "[FAIL] {RequestName} failed after {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

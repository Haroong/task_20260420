using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using task_20260420.Common.Behaviors;
using task_20260420.Common.Exceptions;

namespace task_20260420.Tests.Unit.Common;

public class LoggingBehaviorTests
{
    private readonly FakeLogger<LoggingBehavior<TestRequest, string>> _logger = new();
    private readonly LoggingBehavior<TestRequest, string> _sut;

    public LoggingBehaviorTests()
    {
        _sut = new LoggingBehavior<TestRequest, string>(_logger);
    }

    [Fact]
    public async Task Handle_WhenHandlerThrows_LogsErrorAndRethrows()
    {
        var request = new TestRequest("fail");
        var expectedException = new InvalidOperationException("handler exploded");

        RequestHandlerDelegate<string> next =
            _ => Task.FromException<string>(expectedException);
        var act = () => _sut.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("handler exploded");

        _logger.Should()
            .ContainEntry(LogLevel.Error, "TestRequest");
    }

    [Fact]
    public async Task Handle_WhenAppExceptionThrown_LogsWarningNotError()
    {
        var request = new TestRequest("not-found");
        var appException = new NotFoundException("직원을 찾을 수 없습니다.");

        RequestHandlerDelegate<string> next =
            _ => Task.FromException<string>(appException);
        var act = () => _sut.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        _logger.Should()
            .ContainEntry(LogLevel.Warning, "TestRequest");
        _logger.Should()
            .NotContainEntry(LogLevel.Error);
    }

    [Fact]
    public async Task Handle_WhenHandlerSucceeds_DoesNotLogError()
    {
        var request = new TestRequest("ok");

        RequestHandlerDelegate<string> next = _ => Task.FromResult("done");
        await _sut.Handle(request, next, CancellationToken.None);

        _logger.Should()
            .NotContainEntry(LogLevel.Error);
    }

    public record TestRequest(string Value) : IRequest<string>;
}

internal sealed class FakeLogger<T> : ILogger<T>
{
    private readonly List<(LogLevel Level, string Message)> _entries = [];

    public IReadOnlyList<(LogLevel Level, string Message)> Entries => _entries;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _entries.Add((logLevel, formatter(state, exception)));
    }

    public bool IsEnabled(LogLevel logLevel) => true;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}

internal sealed class FakeLoggerAssertions
{
    private readonly FakeLogger<LoggingBehavior<LoggingBehaviorTests.TestRequest, string>> _logger;

    public FakeLoggerAssertions(
        FakeLogger<LoggingBehavior<LoggingBehaviorTests.TestRequest, string>> logger)
    {
        _logger = logger;
    }

    public void ContainEntry(LogLevel level, string substring)
    {
        _logger.Entries.Should().Contain(e =>
            e.Level == level && e.Message.Contains(substring));
    }

    public void NotContainEntry(LogLevel level)
    {
        _logger.Entries.Should().NotContain(e => e.Level == level);
    }
}

internal static class FakeLoggerExtensions
{
    public static FakeLoggerAssertions Should(
        this FakeLogger<LoggingBehavior<LoggingBehaviorTests.TestRequest, string>> logger)
    {
        return new FakeLoggerAssertions(logger);
    }
}

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.TestExtensions;

/// <summary>
/// Routes <see cref="ILogger{T}"/> output to xUnit's <see cref="ITestOutputHelper"/>.
/// </summary>
public sealed class TestOutputLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        output.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        if (exception != null)
        {
            output.WriteLine(exception.ToString());
        }
    }
}

using Microsoft.Extensions.Logging;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Utilities;

internal sealed class ConsoleLogger<TCategory> : ILogger<TCategory>
{
    private readonly LogLevel _minLevel;

    public ConsoleLogger(LogLevel minLevel = LogLevel.Debug)
    {
        _minLevel = minLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        Console.WriteLine(
            $"{DateTime.UtcNow:O} [{logLevel}] {typeof(TCategory).Name}: {message}");
        if (exception is not null)
        {
            Console.WriteLine(exception);
        }
    }
}

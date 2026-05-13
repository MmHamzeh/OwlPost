using Microsoft.Extensions.Logging;
using OwlPost.Core.ServicesContract;
using OwlPost.Core.Utilities;

namespace OwlPost.Core.Services;

internal class AppLogger<T> : IAppLogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {

        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);

        //Log the message to the console for now, but this can be extended to log to a file or other logging providers
        Console.WriteLine(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (!ApplicationSetting.IsLogEnabled)
            return false;

        return logLevel switch
        {
            LogLevel.Trace or LogLevel.Debug => ApplicationSetting.IsDebugMode,
            LogLevel.Information => true,
            LogLevel.Warning => true,
            LogLevel.Error => true,
            LogLevel.Critical => true,
            LogLevel.None => false,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}

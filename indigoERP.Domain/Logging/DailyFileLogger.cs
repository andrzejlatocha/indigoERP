using System.Text;
using Microsoft.Extensions.Logging;

namespace indigoERP.Domain.Logging;

public static class DailyFileLoggerExtensions
{
    /// <summary>Dodaje logowanie do plików dziennych: {logDirectory}\{applicationName}-{rrrr-MM-dd}.log.</summary>
    public static ILoggingBuilder AddDailyFile(this ILoggingBuilder builder, string logDirectory, string applicationName,
        LogLevel minLevel = LogLevel.Information)
    {
        builder.AddProvider(new DailyFileLoggerProvider(logDirectory, applicationName, minLevel));
        return builder;
    }

    /// <summary>Katalog logów z konfiguracji (Logging:FilePath); ścieżka względna liczona od katalogu aplikacji, brak = podkatalog Logs.</summary>
    public static string ResolveLogDirectory(string? configuredPath, string contentRootPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath)) return Path.Combine(contentRootPath, "Logs");
        return Path.IsPathRooted(configuredPath) ? configuredPath : Path.GetFullPath(Path.Combine(contentRootPath, configuredPath));
    }
}

public sealed class DailyFileLoggerProvider : ILoggerProvider
{
    private readonly string _logDirectory;
    private readonly string _applicationName;
    private readonly LogLevel _minLevel;

    public DailyFileLoggerProvider(string logDirectory, string applicationName, LogLevel minLevel)
    {
        _logDirectory = logDirectory;
        _applicationName = applicationName;
        _minLevel = minLevel;

        // Brak uprawnień do katalogu logów nie może zatrzymać aplikacji; zapis pojedynczego wpisu też jest zabezpieczony.
        try { Directory.CreateDirectory(_logDirectory); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
    }

    public ILogger CreateLogger(string categoryName) => new DailyFileLogger(_logDirectory, _applicationName, categoryName, _minLevel);

    public void Dispose()
    {
    }
}

internal sealed class DailyFileLogger(string logDirectory, string applicationName, string categoryName, LogLevel minLevel) : ILogger
{
    private static readonly object LockObject = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && logLevel >= minLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        string message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message) && exception is null) return;

        DateTime now = DateTime.Now;
        string filePath = Path.Combine(logDirectory, $"{applicationName}-{now:yyyy-MM-dd}.log");
        var builder = new StringBuilder();

        builder.Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(" [").Append(logLevel).Append("] ").Append(categoryName);
        if (eventId.Id != 0 || !string.IsNullOrWhiteSpace(eventId.Name))
        {
            builder.Append(" EventId=").Append(eventId.Id);
            if (!string.IsNullOrWhiteSpace(eventId.Name)) builder.Append('/').Append(eventId.Name);
        }

        if (!string.IsNullOrWhiteSpace(message)) builder.AppendLine().Append(message);
        if (exception is not null) builder.AppendLine().Append(exception);
        builder.AppendLine().AppendLine();

        try
        {
            lock (LockObject)
            {
                Directory.CreateDirectory(logDirectory);
                File.AppendAllText(filePath, builder.ToString(), new UTF8Encoding(true));
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Logowanie nigdy nie przerywa obsługi żądania.
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}

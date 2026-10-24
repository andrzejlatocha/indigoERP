using System.Text;
using Microsoft.Extensions.Logging;

namespace indigoERP.Worker.Services;

public sealed class DailyFileLoggerProvider : ILoggerProvider
{
    private readonly object _sync = new();
    private readonly string _directory;
    private readonly LogLevel _minimumLevel;
    private readonly int _retentionDays;

    public DailyFileLoggerProvider(string directory, LogLevel minimumLevel, int retentionDays)
    {
        _directory = directory;
        _minimumLevel = minimumLevel;
        _retentionDays = retentionDays;
    }

    public ILogger CreateLogger(string categoryName) => new DailyFileLogger(this, categoryName);

    public void Dispose()
    {
    }

    private void Write(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception? exception)
    {
        try
        {
            var now = DateTime.Now;
            var path = Path.Combine(_directory, $"indigoERP.Worker-{now:yyyyMMdd}.log");
            var content = new StringBuilder();
            content.Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            content.Append(" [");
            content.Append(logLevel);
            content.Append("] ");
            content.Append(categoryName);
            content.Append(" (event ");
            content.Append(eventId.Id);
            content.Append("): ");
            content.AppendLine(message);

            if (exception != null)
            {
                content.AppendLine(exception.ToString());
            }

            lock (_sync)
            {
                Directory.CreateDirectory(_directory);
                DeleteExpiredLogs(now.Date);
                File.AppendAllText(path, content.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            }
        }
        catch
        {
            // Błąd zapisu logu nie może przerwać przetwarzania kolejki wiadomości.
        }
    }

    private void DeleteExpiredLogs(DateTime today)
    {
        var cutoff = today.AddDays(-_retentionDays);

        foreach (var file in Directory.EnumerateFiles(_directory, "indigoERP.Worker-*.log"))
        {
            if (File.GetLastWriteTime(file).Date < cutoff)
            {
                File.Delete(file);
            }
        }
    }

    private sealed class DailyFileLogger : ILogger
    {
        private readonly DailyFileLoggerProvider _provider;
        private readonly string _categoryName;

        public DailyFileLogger(DailyFileLoggerProvider provider, string categoryName)
        {
            _provider = provider;
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _provider._minimumLevel && logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                _provider.Write(logLevel, _categoryName, eventId, formatter(state, exception), exception);
            }
        }
    }
}

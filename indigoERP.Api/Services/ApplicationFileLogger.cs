using System.Text;

namespace indigoERP.Api.Services;

internal static class ApplicationFileLogger
{
    private static readonly object Sync = new();

    public static void LogError(string category, string message, Exception exception)
    {
        try
        {
            DateTime now = DateTime.Now;
            string directory = Path.Combine(AppContext.BaseDirectory, "Logs");
            string path = Path.Combine(directory, $"application-errors-{now:yyyy-MM}.log");
            var content = new StringBuilder();
            content.AppendLine(new string('-', 120));
            content.AppendLine($"DATE: {now:yyyy-MM-dd HH:mm:ss.fff}");
            content.AppendLine($"APPLICATION: {AppDomain.CurrentDomain.FriendlyName}");
            content.AppendLine($"MACHINE: {Environment.MachineName}");
            content.AppendLine($"TYPE: {category}");
            content.AppendLine($"MESSAGE: {message}");
            content.AppendLine(exception.ToString());

            lock (Sync)
            {
                Directory.CreateDirectory(directory);
                File.AppendAllText(path, content.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            }
        }
        catch
        {
            // Błąd zapisu logu nie może ukryć pierwotnego błędu operacji.
        }
    }
}

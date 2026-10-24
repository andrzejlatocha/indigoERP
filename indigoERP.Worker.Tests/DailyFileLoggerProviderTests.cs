using indigoERP.Worker.Services;
using Microsoft.Extensions.Logging;

namespace indigoERP.Worker.Tests;

public sealed class DailyFileLoggerProviderTests
{
    [Fact]
    public void Logger_WritesInformationMessageToDailyFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            using var provider = new DailyFileLoggerProvider(directory, LogLevel.Information, 30);
            var logger = provider.CreateLogger("WorkerTests");

            logger.LogInformation("Kolejka została uruchomiona.");

            var path = Path.Combine(directory, $"indigoERP.Worker-{DateTime.Now:yyyyMMdd}.log");
            Assert.True(File.Exists(path));
            Assert.Contains("Kolejka została uruchomiona.", File.ReadAllText(path));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}

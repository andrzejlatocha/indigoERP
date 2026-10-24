using indigoERP.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscMiniaturaServiceTests : IDisposable
{
    private const string MinimalnyPdf =
        "%PDF-1.4\n" +
        "1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n" +
        "2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n" +
        "3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 200 300]/Contents 4 0 R/Resources<<>>>>endobj\n" +
        "4 0 obj<</Length 44>>stream\n" +
        "BT /F1 12 Tf 20 250 Td (Test) Tj ET\n" +
        "endstream endobj\n" +
        "trailer<</Root 1 0 R>>\n" +
        "%%EOF\n";

    private readonly string _katalog = Path.Combine(Path.GetTempPath(), "miniatura-test-" + Guid.NewGuid().ToString("N"));

    public WiadomoscMiniaturaServiceTests()
    {
        Directory.CreateDirectory(_katalog);
    }

    public void Dispose()
    {
        if (Directory.Exists(_katalog))
        {
            Directory.Delete(_katalog, recursive: true);
        }
    }

    [Fact]
    public void PobierzMiniature_renderuje_pierwsza_strone_pdf_jako_jpeg()
    {
        var sciezkaPdf = Path.Combine(_katalog, "numer.pdf");
        File.WriteAllText(sciezkaPdf, MinimalnyPdf);
        var logger = new ListLogger<WiadomoscMiniaturaService>();
        var serwis = new WiadomoscMiniaturaService(logger);

        var miniatura = serwis.PobierzMiniature(sciezkaPdf);

        Assert.True(miniatura != null, "Blad: " + string.Join(" | ", logger.Wyjatki));
        Assert.True(File.Exists(miniatura));
        var bajty = File.ReadAllBytes(miniatura);
        Assert.Equal(0xFF, bajty[0]);
        Assert.Equal(0xD8, bajty[1]);
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<string> Wyjatki { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (exception != null)
            {
                Wyjatki.Add(exception.GetType().Name + ": " + exception.Message + " @ " + exception.StackTrace?.Split('\n').FirstOrDefault());
            }
        }
    }

    [Fact]
    public void PobierzMiniature_zwraca_null_dla_pliku_ktory_nie_jest_pdf()
    {
        var sciezkaPdf = Path.Combine(_katalog, "smiec.pdf");
        File.WriteAllText(sciezkaPdf, "to nie jest pdf");
        var serwis = new WiadomoscMiniaturaService(NullLogger<WiadomoscMiniaturaService>.Instance);

        var miniatura = serwis.PobierzMiniature(sciezkaPdf);

        Assert.Null(miniatura);
        Assert.False(File.Exists(sciezkaPdf + ".miniatura.jpg"));
    }
}

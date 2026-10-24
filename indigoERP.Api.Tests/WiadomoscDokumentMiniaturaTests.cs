using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Api.Data;
using indigoERP.Api.Services;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscDokumentMiniaturaTests : IDisposable
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

    private readonly string _katalog = Path.Combine(Path.GetTempPath(), "dokument-miniatura-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_katalog))
        {
            Directory.Delete(_katalog, recursive: true);
        }
    }

    [Fact]
    public async Task MiniaturaPubliczna_zwraca_jpeg_dla_pdf_publicznej_wiadomosci()
    {
        await using var context = ApiTestContextFactory.Create();
        SeedDokument(context, wiaPubliczna: true, wdoID: 1);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.MiniaturaPubliczna(1);

        var plik = Assert.IsType<PhysicalFileResult>(result);
        Assert.Equal("image/jpeg", plik.ContentType);
    }

    [Fact]
    public async Task MiniaturaPubliczna_nie_zwraca_miniatury_dla_wiadomosci_prywatnej()
    {
        await using var context = ApiTestContextFactory.Create();
        SeedDokument(context, wiaPubliczna: false, wdoID: 2);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.MiniaturaPubliczna(2);

        Assert.IsType<NotFoundResult>(result);
    }

    private WiadomoscDokumentController CreateController(indigoERPContext context)
    {
        var konfiguracja = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["FileStorage:Path"] = _katalog })
            .Build();

        return new WiadomoscDokumentController(
            context,
            new TestWebHostEnvironment(_katalog),
            konfiguracja,
            new WiadomoscMiniaturaService(NullLogger<WiadomoscMiniaturaService>.Instance));
    }

    private void SeedDokument(indigoERPContext context, bool wiaPubliczna, int wdoID)
    {
        var katalog = Path.Combine("GLOS", "000000000101", "20260101000000");
        var folder = Path.Combine(_katalog, katalog);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "numer.pdf"), MinimalnyPdf);

        var wiadomoscID = 100 + wdoID;
        context.Wiadomosc.Add(new Wiadomosc
        {
            wiaID = wiadomoscID,
            wiaAplikacjaID = 900,
            wiaTemat = "Numer testowy",
            wiaTresc = "<p>Treść</p>",
            wiaData = DateTime.Now,
            wiaStatus = "2",
            wiaPubliczna = wiaPubliczna
        });
        context.WiadomoscDokument.Add(new WiadomoscDokument
        {
            wdoID = wdoID,
            wdoWiadomoscID = wiadomoscID,
            wdoTyp = "pdf",
            wdoNazwa = "numer.pdf",
            wdoPlik = "numer.pdf",
            wdoKatalog = katalog,
            wdoStatus = "2"
        });
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = new NullFileProvider();
            WebRootFileProvider = new NullFileProvider();
        }

        public string ApplicationName { get; set; } = "indigoERP.Api.Tests";
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; } = "Development";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; }
    }
}

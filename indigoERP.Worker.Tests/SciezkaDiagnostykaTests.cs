using indigoERP.Worker.Services;

namespace indigoERP.Worker.Tests;

public class SciezkaDiagnostykaTests
{
    [Fact]
    public void Opisz_ExistingFile_ReportsReadableFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"diagnostyka-{Guid.NewGuid():N}.pdf");
        File.WriteAllBytes(path, [1, 2, 3]);

        try
        {
            var opis = SciezkaDiagnostyka.Opisz(path);

            Assert.Contains("File.Exists: True", opis);
            Assert.Contains("Próba odczytu: OK", opis);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Opisz_MissingFile_ReportsMissingFileAndFolderState()
    {
        var path = Path.Combine(Path.GetTempPath(), $"brak-{Guid.NewGuid():N}", "plik.pdf");

        var opis = SciezkaDiagnostyka.Opisz(path);

        Assert.Contains("File.Exists: False", opis);
        Assert.Contains($"Katalog False: {Path.GetDirectoryName(path)}", opis);
    }
}

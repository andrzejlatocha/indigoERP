using PDFtoImage;
using SkiaSharp;

namespace indigoERP.Api.Services
{
    /// <summary>Miniatura pierwszej strony załącznika PDF (JPEG) — generowana przy pierwszym żądaniu i zapisywana obok pliku.</summary>
    public interface IWiadomoscMiniaturaService
    {
        /// <summary>Zwraca ścieżkę do miniatury albo null, gdy PDF nie daje się wyrenderować.</summary>
        string? PobierzMiniature(string sciezkaPdf);
    }

    public sealed class WiadomoscMiniaturaService : IWiadomoscMiniaturaService
    {
        private const int SzerokoscPx = 480;
        private const string Rozszerzenie = ".miniatura.jpg";

        private readonly ILogger<WiadomoscMiniaturaService> _logger;

        public WiadomoscMiniaturaService(ILogger<WiadomoscMiniaturaService> logger)
        {
            _logger = logger;
        }

        public string? PobierzMiniature(string sciezkaPdf)
        {
            var sciezkaMiniatury = sciezkaPdf + Rozszerzenie;

            if (File.Exists(sciezkaMiniatury))
            {
                return sciezkaMiniatury;
            }

            // Zapis przez plik tymczasowy: równoległe żądania nie zobaczą pliku w połowie zapisu.
            var tymczasowa = sciezkaMiniatury + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                using (var wyjscie = new FileStream(tymczasowa, FileMode.CreateNew))
                {
                    var opcje = new RenderOptions { Width = SzerokoscPx, WithAspectRatio = true };
                    Conversion.SaveJpeg(wyjscie, File.ReadAllBytes(sciezkaPdf), 0, null, opcje);
                }

                File.Move(tymczasowa, sciezkaMiniatury, overwrite: true);

                return sciezkaMiniatury;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nie udało się przygotować miniatury PDF: {Sciezka}", sciezkaPdf);

                if (File.Exists(tymczasowa))
                {
                    File.Delete(tymczasowa);
                }

                return null;
            }
        }
    }
}

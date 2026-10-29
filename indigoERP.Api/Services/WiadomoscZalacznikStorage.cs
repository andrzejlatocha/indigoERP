using indigoERP.Api.Services;
using Microsoft.AspNetCore.Http;

namespace indigoERP.Api.Services
{
    /// <summary>Zapis załączników wysyłki w katalogu FileStorage:Path (ten sam katalog co dokumenty wiadomości).</summary>
    public interface IWiadomoscZalacznikStorage
    {
        Task<(string Katalog, string Plik)> ZapiszPlik(IFormFile plik, string appFolder, string usrFolder, CancellationToken cancellationToken);
    }

    public sealed class WiadomoscZalacznikStorage : IWiadomoscZalacznikStorage
    {
        public const long MaxRozmiarPliku = 30L * 1024 * 1024;
        public const long MaxLacznyRozmiar = 60L * 1024 * 1024;
        public const int MaxPlikow = 5;

        private readonly string _root;

        public WiadomoscZalacznikStorage(string root)
        {
            _root = root;
        }

        public static string Root(Microsoft.Extensions.Configuration.IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            var root = configuration["FileStorage:Path"];

            if (string.IsNullOrWhiteSpace(root))
            {
                root = Path.Combine(environment.ContentRootPath, "wwwroot", "_documents");
            }

            return Path.GetFullPath(root);
        }

        /// <summary>Zwraca komunikat błędu albo null, gdy plik jest akceptowalny (PDF, do limitu rozmiaru, poprawny nagłówek).</summary>
        public static string? Waliduj(IFormFile plik)
        {
            if (plik.Length == 0)
            {
                return $"Plik „{plik.FileName}” jest pusty.";
            }

            if (plik.Length > MaxRozmiarPliku)
            {
                return $"Plik „{plik.FileName}” przekracza 30 MB.";
            }

            if (!string.Equals(Path.GetExtension(plik.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return $"Plik „{plik.FileName}”: dozwolone są tylko pliki PDF.";
            }

            using var stream = plik.OpenReadStream();
            var naglowek = new byte[5];
            var odczytano = stream.Read(naglowek, 0, naglowek.Length);

            if (odczytano < 4 || naglowek[0] != '%' || naglowek[1] != 'P' || naglowek[2] != 'D' || naglowek[3] != 'F')
            {
                return $"Plik „{plik.FileName}” nie jest poprawnym plikiem PDF.";
            }

            return null;
        }

        public async Task<(string Katalog, string Plik)> ZapiszPlik(
            IFormFile plik, string appFolder, string usrFolder, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_root))
            {
                throw new InvalidOperationException("Nie skonfigurowano katalogu plików (FileStorage:Path).");
            }

            // Wzorzec epicaHR: {appFolder}\{usrFolder}\{yyyyMMddHHmmss}\{plik}.
            var katalog = Path.Combine(Segment(appFolder), Segment(usrFolder), DateTime.Now.ToString("yyyyMMddHHmmss"));
            var folder = Path.Combine(_root, katalog);
            Directory.CreateDirectory(folder);

            var nazwa = $"{Guid.NewGuid():N}_{Sanitize(Path.GetFileName(plik.FileName))}";
            var docelowa = Path.Combine(folder, nazwa);

            await using var stream = new FileStream(docelowa, FileMode.CreateNew);
            await plik.CopyToAsync(stream, cancellationToken);

            return (katalog, nazwa);
        }

        /// <summary>Segment ścieżki bez separatorów i znaków niedozwolonych; pusta wartość daje "_".</summary>
        public static string Segment(string? wartosc)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var czysty = new string((wartosc ?? string.Empty)
                .Where(c => !invalid.Contains(c) && c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar)
                .ToArray()).Trim();

            return string.IsNullOrEmpty(czysty) ? "_" : czysty;
        }

        private static string Sanitize(string nazwa)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return new string(nazwa.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        }
    }
}
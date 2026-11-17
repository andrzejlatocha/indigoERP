using System;

namespace indigoERP.Domain.Common
{
    /// <summary>
    /// Linki z powiadomień: wyłącznie ścieżki względne aplikacji (zaczynające się od pojedynczego "/")
    /// oraz adresy bezwzględne http i https. Blokuje javascript:, data: i pozostałe schematy.
    /// </summary>
    public static class BezpieczneLinki
    {
        public static bool Dozwolony(string? link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return true;
            }

            var wartosc = link.Trim();

            // "//adres" i ścieżki z "\" to adresy zewnętrzne lub obejścia walidacji, więc odrzucamy je przed sprawdzeniem schematu.
            if (wartosc.StartsWith('/') && !wartosc.StartsWith("//", StringComparison.Ordinal) && !wartosc.Contains('\\'))
            {
                return true;
            }

            return Uri.TryCreate(wartosc, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}

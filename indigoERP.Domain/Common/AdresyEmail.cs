using System.Net.Mail;
using System.Text.RegularExpressions;

namespace indigoERP.Domain.Common
{
    /// <summary>
    /// Walidacja własnej listy adresów e-mail (tagbox w oknie wysyłki). Ta sama reguła po stronie Blazora i API.
    /// Adresy są przycinane, puste pomijane, a duplikaty (bez względu na wielkość liter) usuwane.
    /// </summary>
    public static class AdresyEmail
    {
        public const int MaksymalnaLiczba = 200;
        public const int MaksymalnaDlugosc = 254;

        private static readonly Regex Wzorzec = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Zwraca poprawne, unikalne adresy albo komunikat o pierwszym błędzie.</summary>
        public static (List<string> Adresy, string? Blad) Waliduj(IEnumerable<string>? adresy)
        {
            var oczyszczone = (adresy ?? Enumerable.Empty<string>())
                .Select(a => a?.Trim() ?? string.Empty)
                .Where(a => a.Length > 0)
                .DistinctBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (oczyszczone.Count == 0)
            {
                return ([], "Podaj co najmniej jeden adres e-mail.");
            }

            if (oczyszczone.Count > MaksymalnaLiczba)
            {
                return ([], $"Można podać najwyżej {MaksymalnaLiczba} adresów.");
            }

            foreach (var adres in oczyszczone)
            {
                if (!CzyPoprawny(adres))
                {
                    return ([], $"Nieprawidłowy adres e-mail: {adres}");
                }
            }

            return (oczyszczone, null);
        }

        /// <summary>Sprawdza jeden adres: długość, format i poprawność według MailAddress.</summary>
        public static bool CzyPoprawny(string adres)
        {
            if (string.IsNullOrWhiteSpace(adres) || adres.Length > MaksymalnaDlugosc || !Wzorzec.IsMatch(adres))
            {
                return false;
            }

            try
            {
                return new MailAddress(adres).Address.Equals(adres, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

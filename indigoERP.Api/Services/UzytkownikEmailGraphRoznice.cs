using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Administrator;

namespace indigoERP.Api.Services
{
    /// <summary>
    /// Porównanie adresów e-mail użytkowników indigoERP z kontami Microsoft 365 (za źródło prawdy przyjmujemy M365):
    /// Dodaj (pusty adres, jest w M365), Popraw (adres różni się od M365), Usun (konto nieaktywne, status = '1',
    /// albo aktywne z adresem, którego nie ma w M365 - czyścimy tylko adres, rekord zostaje).
    /// </summary>
    public sealed class UzytkownikEmailGraphRoznice
    {
        private readonly Dictionary<string, List<GraphUserInfo>> _bySam;
        private readonly Dictionary<string, List<GraphUserInfo>> _byUpn;

        public UzytkownikEmailGraphRoznice(IEnumerable<GraphUserInfo> graphUsers)
        {
            var list = graphUsers.ToList();
            _bySam = list
                .Where(g => !string.IsNullOrWhiteSpace(g.OnPremisesSamAccountName))
                .GroupBy(g => g.OnPremisesSamAccountName!.Trim().ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.ToList());
            _byUpn = list
                .Where(g => !string.IsNullOrWhiteSpace(g.UserPrincipalName))
                .GroupBy(g => LoginPart(g.UserPrincipalName))
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>Klasyfikuje jednego użytkownika; zwraca model z polami wynik i akcja.</summary>
        public UzytkownikEmailGraphModel Klasyfikuj(Uzytkownik u)
        {
            var pozycja = new UzytkownikEmailGraphModel
            {
                usrID = u.usrID,
                usrNazwa = u.usrNazwa,
                usrNazwaAD = u.usrNazwaAD,
                usrImieNazwisko = u.usrImieNazwisko,
                usrStatus = u.usrStatus,
                usrEmail = u.usrEmail
            };

            var obecny = (u.usrEmail ?? string.Empty).Trim();
            var nieaktywny = u.usrStatus == "1";

            // Reguła: konto ze statusem 1 nie powinno mieć adresu e-mail.
            if (nieaktywny)
            {
                if (obecny.Length > 0)
                {
                    pozycja.akcja = UzytkownikEmailGraphAkcja.Usun;
                }

                return pozycja;
            }

            GraphUserInfo? znaleziony = null;

            foreach (var key in new[] { LoginPart(u.usrNazwaAD), LoginPart(u.usrNazwa) }.Where(k => k.Length > 0).Distinct())
            {
                var kandydaci = (_bySam.TryGetValue(key, out var sam) ? sam : _byUpn.TryGetValue(key, out var upn) ? upn : null)
                    ?.DistinctBy(k => k.Id).ToList();

                if (kandydaci == null || kandydaci.Count == 0)
                {
                    continue;
                }

                if (kandydaci.Count > 1)
                {
                    pozycja.wynik = "Niejednoznaczne";
                    return pozycja;
                }

                znaleziony = kandydaci[0];
                break;
            }

            if (znaleziony == null)
            {
                // Brak konta w M365 przy aktywnym użytkowniku z adresem: czyścimy tylko adres e-mail.
                // Rekordu nie kasujemy - usuwa go wyłącznie synchronizacja z epicaHR.
                pozycja.wynik = "Brak";

                if (obecny.Length > 0)
                {
                    pozycja.akcja = UzytkownikEmailGraphAkcja.Usun;
                }

                return pozycja;
            }

            var mail = (znaleziony.Mail ?? string.Empty).Trim();
            var adres = mail.Length > 0 ? mail : (znaleziony.UserPrincipalName ?? string.Empty).Trim();

            if (adres.Length == 0)
            {
                return pozycja;
            }

            pozycja.graphEmail = adres;
            pozycja.graphDisplayName = znaleziony.DisplayName;
            pozycja.graphUserPrincipalName = znaleziony.UserPrincipalName;
            pozycja.wynik = "Dopasowano";

            if (obecny.Length == 0)
            {
                pozycja.akcja = UzytkownikEmailGraphAkcja.Dodaj;
            }
            else if (mail.Length > 0 && !string.Equals(obecny, mail, StringComparison.OrdinalIgnoreCase))
            {
                // Nadpisujemy istniejący adres tylko prawdziwą skrzynką (mail), nigdy samym UPN.
                pozycja.akcja = UzytkownikEmailGraphAkcja.Popraw;
            }

            return pozycja;
        }

        /// <summary>Czy pozycja ma być pokazana w oknie: ma akcję albo jest nierozstrzygniętym użytkownikiem bez adresu.</summary>
        public static bool DoPokazania(UzytkownikEmailGraphModel p) =>
            p.akcja.Length > 0 || (string.IsNullOrWhiteSpace(p.usrEmail) && p.usrStatus != "1");

        // Login do porównań: bez domeny (DOMENA\login, login@domena), małymi literami.
        public static string LoginPart(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var login = value.Trim();
            var slash = login.LastIndexOf('\\');

            if (slash >= 0)
            {
                login = login[(slash + 1)..];
            }

            var at = login.IndexOf('@');

            return (at >= 0 ? login[..at] : login).ToLowerInvariant();
        }
    }
}

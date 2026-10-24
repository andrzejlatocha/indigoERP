namespace indigoERP.Domain.Models._Administrator
{
    /// <summary>
    /// Różnica między adresem e-mail użytkownika w indigoERP a kontem w Microsoft 365 (Microsoft Graph).
    /// </summary>
    public class UzytkownikEmailGraphModel
    {
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string? usrStatus { get; set; }

        /// <summary>Adres zapisany obecnie w indigoERP (Uzytkownik.usrEmail).</summary>
        public string? usrEmail { get; set; }

        /// <summary>Adres z Microsoft 365 (mail, a przy dodawaniu - gdy pusty - userPrincipalName).</summary>
        public string? graphEmail { get; set; }
        public string? graphDisplayName { get; set; }
        public string? graphUserPrincipalName { get; set; }

        /// <summary>Dopasowano | Niejednoznaczne | Brak</summary>
        public string wynik { get; set; } = "Brak";

        /// <summary>Proponowana zmiana: Dodaj | Popraw | Usun; pusta, gdy nie ma czego zmieniać.</summary>
        public string akcja { get; set; } = string.Empty;
    }

    public static class UzytkownikEmailGraphAkcja
    {
        public const string Dodaj = "Dodaj";
        public const string Popraw = "Popraw";
        public const string Usun = "Usun";
    }
}

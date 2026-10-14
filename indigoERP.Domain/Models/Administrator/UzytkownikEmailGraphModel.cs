namespace indigoERP.Domain.Models._Administrator
{
    /// <summary>
    /// Wynik dopasowania użytkownika indigoERP bez adresu e-mail do konta Microsoft 365 (Microsoft Graph).
    /// Tylko odczyt - nic nie jest zapisywane w bazie.
    /// </summary>
    public class UzytkownikEmailGraphModel
    {
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string? usrImieNazwisko { get; set; }

        /// <summary>Adres znaleziony w Microsoft 365 (mail, a gdy pusty - userPrincipalName).</summary>
        public string? graphEmail { get; set; }
        public string? graphDisplayName { get; set; }
        public string? graphUserPrincipalName { get; set; }

        /// <summary>Dopasowano | Niejednoznaczne | Brak</summary>
        public string wynik { get; set; } = "Brak";
    }
}

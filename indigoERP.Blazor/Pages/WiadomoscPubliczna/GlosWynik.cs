using Radzen;

namespace indigoERP.Blazor.Pages.WiadomoscPubliczna
{
    /// <summary>
    /// Wynik zamkniętego okna wysyłki. Po sukcesie strona /wiadomosc przeładowuje się, a komunikat jedzie w adresie
    /// (parametry poniżej), bo lista renderowana po stronie serwera odświeża się dopiero po przeładowaniu.
    /// </summary>
    public sealed record GlosWynik(NotificationSeverity Severity, string Tytul, string Tresc)
    {
        public const string ParametrSeverity = "glosSeverity";
        public const string ParametrTytul = "glosTytul";
        public const string ParametrTresc = "glosTresc";
    }
}

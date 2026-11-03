namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Żądanie wysyłki do pracowników wybranego zakresu (tylko odbiorcy z zakresu).</summary>
    public class WiadomoscWysylkaZakresModel
    {
        public int wiaAplikacjaID { get; set; }
        public string wiaTemat { get; set; } = string.Empty;
        public string wiaTresc { get; set; } = string.Empty;

        /// <summary>Wybrany wzorzec szablonu (dbo.WiadomoscWzorzec); brak = domyślny wzorzec Głosu Hutnika.</summary>
        public int? wiaWzorzecID { get; set; }
        public int wzaID { get; set; }
        public List<int> prcIDs { get; set; } = new();
    }

    /// <summary>Wynik wysyłki: utworzona wiadomość, liczba odbiorców i powody pominięcia.</summary>
    public class WiadomoscWysylkaZakresWynikModel
    {
        public int wiaID { get; set; }
        public int dodano { get; set; }
        public int pominieto { get; set; }
        public List<string> komunikaty { get; set; } = new();
    }
}
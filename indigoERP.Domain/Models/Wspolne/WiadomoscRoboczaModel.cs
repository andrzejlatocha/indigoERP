namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Zapis numeru roboczego (bez odbiorców): temat, treść i załączniki wysyłane później.</summary>
    public class WiadomoscRoboczaModel
    {
        public int wiaAplikacjaID { get; set; }
        public string wiaTemat { get; set; } = string.Empty;
        public string wiaTresc { get; set; } = string.Empty;

        /// <summary>Wybrany wzorzec szablonu (dbo.WiadomoscWzorzec); brak = domyślny wzorzec Głosu Hutnika.</summary>
        public int? wiaWzorzecID { get; set; }
    }

    /// <summary>Wysyłka istniejącego numeru roboczego do pracowników wybranego zakresu.</summary>
    public class WiadomoscWyslijRoboczaModel
    {
        public int wzaID { get; set; }
        public List<int> prcIDs { get; set; } = new();
    }
}

namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Zapis numeru roboczego (bez odbiorców): temat, treść i załączniki wysyłane później.</summary>
    public class WiadomoscRoboczaModel
    {
        public int wiaAplikacjaID { get; set; }
        public string wiaTemat { get; set; } = string.Empty;
        public string wiaTresc { get; set; } = string.Empty;

        /// <summary>Wybrany wzorzec szablonu (dbo.WiadomoscWzorzec); brak = domyślny wzorzec wiadomości publicznej.</summary>
        public int? wiaWzorzecID { get; set; }
    }

    /// <summary>Wysyłka istniejącego numeru roboczego do pracowników wybranego zakresu.</summary>
    public class WiadomoscWyslijRoboczaModel
    {
        public int wzaID { get; set; }
        public List<int> prcIDs { get; set; } = new();

        /// <summary>Harmonogram: brak = wysyłka od razu; J = jednorazowo, D = codziennie, T = tygodniowo, M = miesięcznie.</summary>
        public string? kodHarmonogram { get; set; }

        /// <summary>Pierwszy termin wysyłki (wymagany przy harmonogramie).</summary>
        public DateTime? dataHarmonogramStart { get; set; }

        /// <summary>Data końcowa cyklu (opcjonalna; bez niej cykl trwa, aż zostanie anulowany).</summary>
        public DateTime? dataHarmonogramKoniec { get; set; }
    }
}

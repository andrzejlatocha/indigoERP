namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Poprawa wiadomości przed rozpoczęciem wysyłki: temat i treść.</summary>
    public class WiadomoscPoprawaModel
    {
        public string wiaTemat { get; set; } = string.Empty;
        public string? wiaTresc { get; set; }

        /// <summary>Własna lista adresów (tylko wiadomości z zakresem wzaWlasne); null = bez zmian.</summary>
        public List<string>? adresy { get; set; }
    }
}

namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Poprawa wiadomości przed rozpoczęciem wysyłki: temat i treść.</summary>
    public class WiadomoscPoprawaModel
    {
        public string wiaTemat { get; set; } = string.Empty;
        public string? wiaTresc { get; set; }
    }
}

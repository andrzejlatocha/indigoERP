namespace indigoERP.Domain.ViewModels._Wspolne
{
    /// <summary>Aktywny harmonogram wysyłki Głosu Hutnika (szablon wiadomości ze statusem 7).</summary>
    public class WiadomoscHarmonogramViewModel
    {
        public int wiaID { get; set; }
        public string? wiaTemat { get; set; }
        public string? wiaKodHarmonogram { get; set; }
        public string? wiaStatus { get; set; }
        public string? wiaTresc { get; set; }
        public DateTime? wiaDataHarmonogramStart { get; set; }
        public DateTime? wiaDataHarmonogramKoniec { get; set; }
        public DateTime? wiaDataNastepnejWysylki { get; set; }
    }
}

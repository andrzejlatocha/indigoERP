namespace indigoERP.Domain.Dtos._Wspolne
{
    /// <summary>Liczby adresatów wiadomości według statusu wysyłki (dbo.WiadomoscPracownik.wprStatus).</summary>
    public sealed class WiadomoscPodsumowanieDto
    {
        public int wiaID { get; set; }
        /// <summary>Temat, data i status wiadomości (wypełniane tylko przez WiadomoscPracownik/SelectPodsumowanie).</summary>
        public string? wiaTemat { get; set; }
        public DateTime? wiaData { get; set; }
        public string? wiaStatus { get; set; }
        /// <summary>Wszystkie wiersze adresatów.</summary>
        public int Razem { get; set; }
        /// <summary>Status „4” – przyjęte przez system pocztowy.</summary>
        public int Wyslane { get; set; }
        /// <summary>Status „5” – błąd wysyłki albo odpowiedź o niedostarczeniu.</summary>
        public int Bledy { get; set; }
        /// <summary>Status „2” – czeka na wysyłkę.</summary>
        public int Oczekujace { get; set; }
    }
}

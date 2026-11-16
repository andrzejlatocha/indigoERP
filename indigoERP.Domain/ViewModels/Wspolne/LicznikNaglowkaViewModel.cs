namespace indigoERP.Domain.ViewModels._Wspolne
{
    /// <summary>Liczniki nagłówka: nieprzeczytane pozycje zalogowanego użytkownika.</summary>
    public class LicznikNaglowkaViewModel
    {
        public int Powiadomienia { get; set; }
        public int Wiadomosci { get; set; }
        public int Zadania { get; set; }
        public int Zdarzenia { get; set; }
    }
}

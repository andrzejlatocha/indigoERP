namespace indigoERP.Domain.ViewModels._Wspolne
{
    // Przepis wewnętrzny (plik PDF biblioteki Przepisy wewnętrzne, bibID 13) zwracany przez atacamaDMS.Api i proxy indigoERP.Api.
    public class PrzepisWewnetrznyViewModel
    {
        public int pliID { get; set; }
        public string pliNazwa { get; set; } = string.Empty;
        public string? pliOpis { get; set; }
        public DateTime? pliData { get; set; }
    }
}

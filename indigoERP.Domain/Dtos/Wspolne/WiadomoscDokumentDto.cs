namespace indigoERP.Domain.Dtos._Wspolne
{
    public class WiadomoscDokumentDto
    {
        public int wdoID { get; set; }
        public int? wdoWiadomoscID { get; set; }
        public string? wdoTyp { get; set; }
        public string wdoNazwa { get; set; }
        public string wdoPlik { get; set; }
        public string wdoKatalog { get; set; }
        public string wdoLink { get; set; }
        public string wdoStatus { get; set; }
    }
}

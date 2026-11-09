namespace indigoERP.Domain.ViewModels._Wspolne
{
    // Polecenie służbowe obowiązujące, zwracane przez atacamaDMS.Api (PoleceniSluzbowe/SelectPubliczne) i proxy indigoERP.Api.
    public class PolecenieSluzboweViewModel
    {
        public int plsID { get; set; }
        public int plsPlikID { get; set; }
        public string? plsNumer { get; set; }
        public string? plsTytul { get; set; }
        public string? plsRodzajNazwa { get; set; }
        public DateTime? plsDataWydania { get; set; }
        public DateTime plsDataObowiazuje { get; set; }
        public DateTime? plsDataDo { get; set; }
    }
}

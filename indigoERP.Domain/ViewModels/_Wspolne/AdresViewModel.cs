using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vAdresLista", Schema = "mvw")]
    public class AdresViewModel
    {
        [Key]
        public int adrID { get; set; }
        public int adrAdresTypID { get; set; }
        public string? atpGrupa { get; set; }
        public string? atpNazwa { get; set; }
        public DateTime adrDataRozpoczecie { get; set; }
        public DateTime adrDataZakonczenie { get; set; }
        public int? adrKrajID { get; set; }
        public string? kraKod { get; set; }
        public string? kraNazwa { get; set; }
        public int? adrWojewodztwoID { get; set; }
        public string? wojNazwa { get; set; }
	    public int? adrPowiatID { get; set; }
        public string? powNazwa { get; set; }
        public int? adrGminaID { get; set; }
        public string? gmiNazwa { get; set; }
        public string? adrKodPoczta { get; set; }
        public int? adrPocztaID { get; set; }
        public string? pczNazwa { get; set; }
        public int? adrMiejscowoscID { get; set; }
        public string? mscNazwa { get; set; }
        public string? adrUlica { get; set; }
        public string? adrDom { get; set; }
        public string? adrLokal { get; set; }
        public string? adrStatus { get; set; }
        public string? adrAdres { get; set; }
        public string? adrAdresPelny { get; set; }
    }
}

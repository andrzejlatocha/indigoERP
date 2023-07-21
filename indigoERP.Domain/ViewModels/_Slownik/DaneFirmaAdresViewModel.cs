using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vDaneFirmaAdres", Schema = "app")]
    public class DaneFirmaAdresViewModel
    {
        [Key]
        public int fadID { get; set; }
        public int fadDaneFirmaID { get; set; }
        public int adrID { get; set; }
        public string? kraKod { get; set; }
        public string? kraNazwa { get; set; }
        public string? wojNazwa { get; set; }
        public string? powNazwa { get; set; }
        public string? gmiNazwa { get; set; }
        public string? adrKodPoczta { get; set; }
        public string? pczNazwa { get; set; }
        public string? mscNazwa { get; set; }
        public string? adrUlica { get; set; }
        public string? adrDom { get; set; }
        public string? adrLokal { get; set; }
        public string? adrStatus { get; set; }
        public string? adrAdres { get; set; }
        public string? adrAdresPelny { get; set; }
    }
}

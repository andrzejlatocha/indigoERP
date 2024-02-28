using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vZdarzenieLista", Schema = "mvw")]
    public class ZdarzenieViewModel
    {
        [Key]
        public int zdaID { get; set; }
        public int zdaAplikacjaID { get; set; }
        public string? zdaSerwer { get; set; }
        public string? zdaBaza { get; set; }
        public string? zdaTablica { get; set; }
        public int zdaIndeksRekordu { get; set; }
        public string zdaTemat { get; set; }
        public string zdaTresc { get; set; }
        public DateTime zdaData { get; set; }
        public DateTime zdaDataRozpoczecie { get; set; }
        public DateTime zdaDataZakoczenie { get; set; }
        public string zdaStatus { get; set; }
        public int? zurID { get; set; }
        public int? zurUzytkownikID { get; set; }
        public bool? zurOdczyt { get; set; }
        public string? zurStatus { get; set; }
    }
}

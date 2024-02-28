using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.Entities._Wspolne
{
    public class Zdarzenie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int zdaID { get; set; }
        public int zdaAplikacjaID { get; set; }
        public string? zdaSerwer { get; set; }
        public string? zdaBaza { get; set; }
        public string? zdaTablica { get; set; }
        public string zdaTemat { get; set; }
        public string zdaTresc { get; set; }
        public DateTime zdaData { get; set; }
        public DateTime zdaDataRozpoczecie { get; set; }
        public DateTime zdaDataZakoczenie { get; set; }
        public string zdaStatus { get; set; }
    }
}

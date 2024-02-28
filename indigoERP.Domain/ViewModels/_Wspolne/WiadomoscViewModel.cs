using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vWiadomoscLista", Schema = "mvw")]
    public class WiadomoscViewModel
    {
        [Key]
        public int wiaID { get; set; }
        public int wiaWiadomoscID { get; set; }
        public int wiaAplikacjaID { get; set; }
        public int wiaUzytkownikID { get; set; }
        public string wiaTemat { get; set; }
        public string wiaTresc { get; set; }
        public DateTime wiaData { get; set; }
        public string? wiaGrupa { get; set; }
        public string wiaStatus { get; set; }
        public int? wuzID { get; set; }
        public int? wuzUzytkownikID { get; set; }
        public bool? wuzAkceptacja { get; set; }
        public bool? wuzOdczyt { get; set; }
        public string? wuzStatus { get; set; }
    }
}

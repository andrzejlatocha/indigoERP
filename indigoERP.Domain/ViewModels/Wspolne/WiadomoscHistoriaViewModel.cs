using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vWiadomoscHistoria", Schema = "mvw")]
    [Keyless]
    public class WiadomoscHistoriaViewModel
    {
        public int wiaID { get; set; }
        public int? wiaAplikacjaID { get; set; }
        public int? wiaUzytkownikID { get; set; }
        public string? wiaEmail { get; set; }
        public string? wiaTemat { get; set; }
        public string? wiaTresc { get; set; }
        public DateTime? wiaData { get; set; }
        public string? wiaStatus { get; set; }
        public bool? wiaPubliczna { get; set; }
        public int? wwyID { get; set; }
        public DateTime? wwyData { get; set; }
        public string? wwyStatus { get; set; }
    }
}

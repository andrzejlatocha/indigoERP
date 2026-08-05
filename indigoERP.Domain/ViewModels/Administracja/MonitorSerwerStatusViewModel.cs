using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vSerwerStatus", Schema = "mvw")]
    [Keyless]
    public class MonitorSerwerStatusViewModel
    {
        public int srvID { get; set; }
        public string srvNazwa { get; set; }
        public string srvAdres { get; set; }
        public int? srvPort { get; set; }
        public string srvSerwerTypKOD { get; set; }
        public string? styOpis { get; set; }
        public DateTime? slgDataSprawdzenie { get; set; }
        public bool? slgDostepny { get; set; }
        public int? slgCzasOdpowiedzMs { get; set; }
        public string? slgBlad { get; set; }
    }
}

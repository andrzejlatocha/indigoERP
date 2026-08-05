using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vSerwerLista", Schema = "mvw")]
    [Keyless]
    public class SerwerViewModel
    {
        public int srvID { get; set; }
        public string srvSerwerTypKOD { get; set; }
        public string srvNazwa { get; set; }
        public string srvAdres { get; set; }
        public int? srvPort { get; set; }
        public string? srvOpis { get; set; }
        public string? srvStatus { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Common;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Serwer : AuditableEntity
    {
        public Serwer()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int srvID { get; set; }
        public string srvSerwerTypKOD { get; set; }
        public string srvNazwa { get; set; }
        public string srvAdres { get; set; }
        public int? srvPort { get; set; }
        public string srvOpis { get; set; }
        public string srvStatus { get; set; }
    }
}

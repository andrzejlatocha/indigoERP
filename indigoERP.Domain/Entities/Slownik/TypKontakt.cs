using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Slownik
{
    public class TypKontakt : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tkoID { get; set; }
        public int tkoAplikacjaID { get; set; }
        public string tkoIkona { get; set;}
        public string tkoOpis { get; set; }
        public string tkoStatus { get; set; }
    }
}

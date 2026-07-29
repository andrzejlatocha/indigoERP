using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Slownik
{
    public class MarkaPojazd : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int mpjID { get; set; }
        public int? mpjMarkaPojazdID { get; set; }
        public string? mpjNazwa { get; set; }
        public string? mpjStatus { get; set; }
    }
}

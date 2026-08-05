using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("SerwerTyp", Schema = "dic")]
    public partial class SerwerTyp : AuditableEntity
    {
        public SerwerTyp()
        {
        }

        [Key]
        public string styKOD { get; set; }
        public string styOpis { get; set; }
        public string styStatus { get; set; }
    }
}

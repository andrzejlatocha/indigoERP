using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("Powiat", Schema = "dic")]
    public partial class Powiat : AuditableEntity
    {
        public Powiat()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int powID { get; set; }
        public string powNazwa { get; set; }
        public int? powWojewodztwoID { get; set; }
        public string powStatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Gmina", Schema = "dic")]
    public partial class Gmina : AuditableEntity
    {
        public Gmina()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int gmiID { get; set; }
        public string gmiNazwa { get; set; }
        public int? gmiPowiatID { get; set; }
        public string gmiStatus { get; set; }
    }
}

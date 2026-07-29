using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Kraj", Schema = "dic")]
    public partial class Kraj : AuditableEntity
    {
        public Kraj()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kraID { get; set; }
        public string kraKod { get; set; }
        public string kraNazwa { get; set; }
        public string kraStatus { get; set; }
    }
}

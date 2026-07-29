using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Obywatelstwo", Schema = "dic")]
    public partial class Obywatelstwo : AuditableEntity
    {
        public Obywatelstwo()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int obyID { get; set; }
        public string obyOpis { get; set; }
        public string obyStatus { get; set; }
    }
}

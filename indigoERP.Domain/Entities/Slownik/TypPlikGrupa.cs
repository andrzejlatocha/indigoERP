using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("TypPlikGrupa", Schema = "dic")]
    public partial class TypPlikGrupa : AuditableEntity
    {
        public TypPlikGrupa()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tpgID { get; set; }
        public string tpgNazwa { get; set; }
        public string tpgStatus { get; set; }
    }
}

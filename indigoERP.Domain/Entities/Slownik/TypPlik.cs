using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("TypPlik", Schema = "dic")]
    public partial class TypPlik : AuditableEntity
    {
        public TypPlik()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tpkID { get; set; }
        public int tpkAplikacjaID { get; set; }
        public string tpkNazwa { get; set; }
        public string tpkStatus { get; set; }
    }
}

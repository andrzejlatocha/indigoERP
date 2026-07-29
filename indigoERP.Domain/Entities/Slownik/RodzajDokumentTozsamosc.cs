using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("RodzajDokumentTozsamosc", Schema = "dic")]
    public partial class RodzajDokumentTozsamosc : AuditableEntity
    {
        public RodzajDokumentTozsamosc()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int dtyID { get; set; }
        public string dtyOpis { get; set; }
        public string dtyStatus { get; set; }
    }
}

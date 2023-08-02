using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("RodzajDokumentTozsamosc", Schema = "dic")]
    public partial class RodzajDokumentTozsamosc
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

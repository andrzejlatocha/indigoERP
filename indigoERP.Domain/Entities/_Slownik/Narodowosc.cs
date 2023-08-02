using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Narodowosc", Schema = "dic")]
    public partial class Narodowosc
    {
        public Narodowosc()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int narID { get; set; }
        public string narOpis { get; set; }
        public string narStatus { get; set; }
    }
}

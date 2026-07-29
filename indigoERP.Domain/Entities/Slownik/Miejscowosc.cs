using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable enable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Miejscowosc", Schema = "dic")]
    public partial class Miejscowosc : AuditableEntity
    {
        public Miejscowosc()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int mscID { get; set; }
        public string? mscTyp { get; set; }
        public string? mscNazwa { get; set; }
        public int? mscGminaID { get; set; }
        public string? mscStatus { get; set; }
    }
}

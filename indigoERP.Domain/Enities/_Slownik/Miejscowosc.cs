using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class Miejscowosc
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

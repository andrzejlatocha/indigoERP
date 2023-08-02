using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Wojewodztwo", Schema = "dic")]
    public partial class Wojewodztwo
    {
        public Wojewodztwo()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wojID { get; set; }
        public int? wojKrajID { get; set; }
        public string wojNazwa { get; set; }
        public string wojStatus { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace epicaHR.Domain.Entities._Slownik
{
	[Table("Plec", Schema = "dic")]
    public partial class Plec
    {
        public Plec()
        {
        }

        [Key]
        public string pleKOD { get; set; }
        public string pleOpis { get; set; }
        public string pleStatus { get; set; }
    }
}

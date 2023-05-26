using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class Obywatelstwo
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

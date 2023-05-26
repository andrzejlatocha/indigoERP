using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class Powiat
    {
        public Powiat()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int powID { get; set; }
        public string powNazwa { get; set; }
        public int? powWojewodztwoID { get; set; }
        public string powStatus { get; set; }
    }
}

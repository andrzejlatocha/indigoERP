using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Modul
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int apmID { get; set; }
        public int apmAplikacjaID { get; set; }
        public int? apmModulID { get; set; }
        public string? apmNazwa { get; set; }
        public string? apmKatalog { get; set; }
        public string? apmFormularz { get; set; }
        public string? apmIkona { get; set; }
        public string? apmOpis { get; set; }
        public string? apmStatus { get; set; }
    }
}

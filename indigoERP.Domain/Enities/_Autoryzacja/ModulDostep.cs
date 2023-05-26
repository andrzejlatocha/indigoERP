using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class ModulDostep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int aacID { get; set; }
        public int? aacModulID { get; set; }
        public int? aacGrupaID { get; set; }
        public int? aacUzytkownikID { get; set; }
        public int? aacDostepID { get; set; }
        public bool? aacZezwol { get; set; }
        public bool? aacZabron { get; set; }
        public bool? aacDziedziczenie { get; set; }
        public string? aacStatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class AplikacjaDostep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int adoID { get; set; }
        public int? adoAplikacjaID { get; set; }
        public int? adoGrupaID { get; set; }
        public int? adoUzytkownikID { get; set; }
        public int? adoStatus { get; set; }

        public virtual Aplikacja adoAplikacja { get; set; }
        public virtual Grupa adoGrupa { get; set; }
        public virtual Uzytkownik adoUzytkownik { get; set; }
    }
}

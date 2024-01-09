using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class RolaCzlonkowie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cwrID { get; set; }
        public int? cwrUzytkowniID { get; set; }
        public int? cwrGrupaID { get; set; }
        public string cwrStatus { get; set; }

        public virtual Grupa cwrGrupa { get; set; }
        public virtual Uzytkownik cwrUzytkowni { get; set; }
    }
}

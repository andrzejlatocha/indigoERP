using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    [Table("RolaCzlonkowie", Schema = "dbo")]
    public partial class RolaCzlonkowie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cwrID { get; set; }
        public int? cwrUzytkownikID { get; set; }
        public int? cwrRolaID { get; set; }
        public int? cwrGrupaID { get; set; }
        public string? cwrStatus { get; set; }

        public virtual Grupa cwrGrupa { get; set; }
        public virtual Rola cwrRola { get; set; }
        public virtual Uzytkownik cwrUzytkownik { get; set; }
    }
}

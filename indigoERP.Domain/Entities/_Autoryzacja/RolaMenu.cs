using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class RolaMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]		
        public int smrID { get; set; }
        public int? smrMenuID { get; set; }
        public int? smrRolaID { get; set; }
        public bool? smrEdytuj { get; set; }
        public bool? smrAktywne { get; set; }
        public string smrStatus { get; set; }
		
        public virtual Menu smrMenu { get; set; }
        public virtual Rola smrRola { get; set; }
    }
}

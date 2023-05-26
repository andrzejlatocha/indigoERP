using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class Rola
    {
        public Rola()
        {
            InversesrlRola = new HashSet<Rola>();
            RolaMenu = new HashSet<RolaMenu>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int srlID { get; set; }
        public int? srlAplikacjaID { get; set; }
        public string srlNazwa { get; set; }
        public string srlOpis { get; set; }
        public bool? srlBlokada { get; set; }
        public bool? srlEdycja { get; set; }
        public string srlStatus { get; set; }

        public virtual Aplikacja srlAplikacja { get; set; }
        public virtual Rola srlRola { get; set; }
        public virtual ICollection<Rola> InversesrlRola { get; set; }
        public virtual ICollection<RolaMenu> RolaMenu { get; set; }
    }
}

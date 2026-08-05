using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class Menu
    {
        public Menu()
        {
            RolaMenu = new HashSet<RolaMenu>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int mnuID { get; set; }
        public int? mnuMenuID { get; set; }
        public int? mnuModulID { get; set; }
        public byte? mnuKolejnosc { get; set; }
        public string mnuNazwa { get; set; }
        public string mnuObszar { get; set; }
        public string mnuKontroler { get; set; }
        public string mnuAkcja { get; set; }
        public string mnuParametr { get; set; }
        public string mnuStatus { get; set; }

        public virtual ICollection<RolaMenu> RolaMenu { get; set; }
    }
}

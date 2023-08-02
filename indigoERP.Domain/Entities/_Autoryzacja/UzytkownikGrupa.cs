using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class UzytkownikGrupa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]		
        public int uwgID { get; set; }
        public string uwgKluczSortowania { get; set; }
        public int? uwgUzytkownikID { get; set; }
        public int? uwgGrupaID { get; set; }
        public bool? uwgDomyslna { get; set; }
        public string uwgStatus { get; set; }

        public virtual Grupa uwgGrupa { get; set; }
        public virtual Uzytkownik uwgUzytkownik { get; set; }
    }
}

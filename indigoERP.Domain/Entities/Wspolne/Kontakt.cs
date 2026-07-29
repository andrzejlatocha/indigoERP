using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class Kontakt : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kntID { get; set; }
        public int kntAplikacjaID { get; set; }
        [Display(Name = "Typ kontaktu")]
        public int kntTypKontaktID { get; set; }
        [Display(Name = "Kontakt")]
        public string kntKontakt { get; set; }
        public string kntMaska { get; set; }
        [Display(Name = "Uwagi")]
        public string kntUwaga { get; set; }
        public string kntStatus { get; set; }
    }
}

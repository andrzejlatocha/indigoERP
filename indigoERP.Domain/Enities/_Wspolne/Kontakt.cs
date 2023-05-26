using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class Kontakt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kntID { get; set; }
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

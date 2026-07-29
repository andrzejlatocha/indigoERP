using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable enable

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class Adres : AuditableEntity
    {
        public Adres()
        {
            
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int adrID { get; set; }
        [Display(Name = "Typ")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrAdresTypID { get; set; }
        public DateTime adrDataRozpoczecie { get; set; }
        public DateTime adrDataZakonczenie { get; set; }
        [Display(Name = "Kraj")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrKrajID { get; set; }
        [Display(Name = "Województwo")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrWojewodztwoID { get; set; }
        [Display(Name = "Powiat")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrPowiatID { get; set; }
        [Display(Name = "Gmina")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrGminaID { get; set; }
        [Display(Name = "Kod pocztowy")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public string? adrKodPoczta { get; set; }
        [Display(Name = "Poczta")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrPocztaID { get; set; }
        [Display(Name = "Miejscowość")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public int? adrMiejscowoscID { get; set; }
        [Display(Name = "Ulica")]
        public string? adrUlica { get; set; }
        [Display(Name = "Dom")]
        [Required(ErrorMessage = "Pole nie może być puste")]
        public string? adrDom { get; set; }
        [Display(Name = "Lokal")]
        public string? adrLokal { get; set; }
        public string? adrStatus { get; set; }
    }
}

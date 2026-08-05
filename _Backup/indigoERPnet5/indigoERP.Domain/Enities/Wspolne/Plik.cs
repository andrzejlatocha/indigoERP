using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class Plik
    {
        public Plik()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int plkID { get; set; }
        public int? plkUzytkownikID { get; set; }
        public string plkRozszerzenie { get; set; }
        [Display(Name = "Nazwa")]
        public string plkNazwa { get; set; }
        [Display(Name = "Numer")]
        public string plkNumer { get; set; }
        [Display(Name = "Data")]
        public DateTime? plkData { get; set; }
        [Display(Name = "Numer systemowy")]
        public string plkNumerSystem { get; set; }
        [Display(Name = "Pozycja")]
        public string plkPozycja { get; set; }
        [Display(Name = "Archiwum")]
        public string plkArchiwum { get; set; }
        [Display(Name = "Szafa")]
        public string plkSzafa { get; set; }
        [Display(Name = "Półka")]
        public string plkPolka { get; set; }
        [Display(Name = "Teczka")]
        public string plkTeczka { get; set; }
        public string plkPlik { get; set; }
        public string plkLink { get; set; }
        public string plkFolder { get; set; }
        [Display(Name = "Wersja")]
        public byte? plkWersja { get; set; }
        [Display(Name = "Opis")]
        public string plkOpis { get; set; }
        public string plkStatus { get; set; }
    }
}

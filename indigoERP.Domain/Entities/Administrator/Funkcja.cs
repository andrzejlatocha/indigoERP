using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Funkcja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int funID { get; set; }
        public int? funFunkcjaID { get; set; }
        public byte? funKolejnosc { get; set; }
        public string? funNazwa { get; set; }
        public string? funTekst { get; set; }
        public string? funTyp { get; set; }
        public string? funIkona { get; set; }
        public string? funFunkcja { get; set; }
        public string? funStatus { get; set; }
    }
}
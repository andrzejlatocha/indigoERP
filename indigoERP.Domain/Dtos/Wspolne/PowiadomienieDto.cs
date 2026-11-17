using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Wspolne
{
    /// <summary>Dane nowego powiadomienia (zapis przez API). Klucz, data i pola audytowe ustawia API.</summary>
    public class PowiadomienieDto
    {
        [Required]
        public int powAplikacjaID { get; set; }

        [Required]
        public int powUzytkownikID { get; set; }

        [Required]
        [MaxLength(20)]
        public string powKodRodzaj { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string powTytul { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? powTresc { get; set; }

        [MaxLength(500)]
        public string? powLink { get; set; }
    }
}

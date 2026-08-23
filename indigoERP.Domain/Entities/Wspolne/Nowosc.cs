using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("Nowosc", Schema = "dbo")]
    public class Nowosc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int nowID { get; set; }
        public DateTime? nowData { get; set; }
        public int? nowAplikacjaID { get; set; }
        public string? nowURL { get; set; }
        public byte? nowWersja { get; set; }
        public string? nowTytul { get; set; }
        public string? nowOpis { get; set; }
        public string? nowStatus { get; set; }
        public string? nowTypNowoscKOD { get; set; }
    }
}

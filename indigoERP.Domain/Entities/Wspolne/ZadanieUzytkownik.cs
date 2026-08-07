using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("ZadanieUzytkownik", Schema = "dbo")]
    public class ZadanieUzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tusID { get; set; }
        public int? tusZadanieID { get; set; }
        public int? tusUzytkownikID { get; set; }
        public bool? tusOdczyt { get; set; }
        public bool? tusAkceptacja { get; set; }
        public string? tusStatus { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("WiadomoscUzytkownik", Schema = "dbo")]
    public class WiadomoscUzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wuzID { get; set; }
        public int? wuzWiadomoscID { get; set; }
        public int? wuzUzytkownikID { get; set; }
        public bool? wuzAkceptacja { get; set; }
        public bool? wuzOdczyt { get; set; }
        public string? wuzStatus { get; set; }
    }
}

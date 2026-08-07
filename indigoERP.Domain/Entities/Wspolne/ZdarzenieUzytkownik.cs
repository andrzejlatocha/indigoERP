using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("ZdarzenieUzytkownik", Schema = "dbo")]
    public class ZdarzenieUzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int zurID { get; set; }
        public int? zurZdarzenieID { get; set; }
        public int? zurUzytkownikID { get; set; }
        public bool? zurOdczyt { get; set; }
        public string? zurStatus { get; set; }
    }
}

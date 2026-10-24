using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Wspolne;

[Table("WiadomoscPracownik", Schema = "dbo")]
public class WiadomoscPracownik
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int wprID { get; set; }

    public int? wprWiadomoscID { get; set; }
    public int? wprPracownikID { get; set; }
    public string? wprEmail { get; set; }
    public string? wprBlad { get; set; }
    public string? wprStatus { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Wspolne;

public class WiadomoscPracownikDto
{
    public int wprID { get; set; }
    public int? wprWiadomoscID { get; set; }
    public int? wprPracownikID { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? wprEmail { get; set; }

    [StringLength(4000)]
    public string? wprBlad { get; set; }

    [StringLength(1)]
    public string? wprStatus { get; set; }
}

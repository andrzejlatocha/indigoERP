using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Domain.ViewModels._Wspolne;

[Table("Powiadomienie", Schema = "mvw")]
[Keyless]
public sealed class PowiadomienieViewModel
{
    public int powID { get; set; }
    public int powAplikacjaID { get; set; }
    public int powUzytkownikID { get; set; }
    public string powKodRodzaj { get; set; } = string.Empty;
    public string powTytul { get; set; } = string.Empty;
    public string? powTresc { get; set; }
    public string? powLink { get; set; }
    public DateTime powDataPowiadomienia { get; set; }
    public bool powOdczyt { get; set; }
    public DateTime? powDataOdczytu { get; set; }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Wspolne;

[Table("WiadomoscZakres", Schema = "enum")]
[Keyless]
public sealed class WiadomoscZakresEnumerator
{
    public int? wzaID { get; set; }
    public int? wzaAplikacjaID { get; set; }
    public string? wzaNazwa { get; set; }
    public string? wzaWidok { get; set; }
    public string? wzaStatus { get; set; }
    /// <summary>Zakres z własną listą adresów e-mail (tagbox w oknie wysyłki); adresy zapisywane jako wiersze WiadomoscPracownik.</summary>
    public bool? wzaWlasne { get; set; }
}

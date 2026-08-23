using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    // Wąski, celowo ograniczony widok dla strony ogólnodostępnej (bez logowania) —
    // tylko treść biuletynu, bez adresu e-mail czy ID użytkownika. Sam widok
    // mvw.vWiadomoscPubliczna filtruje już po wiaAplikacjaID=900 i wiaPubliczna=1.
    [Table("vWiadomoscPubliczna", Schema = "mvw")]
    [Keyless]
    public class WiadomoscPublicznaViewModel
    {
        public int wiaID { get; set; }
        public string? wiaTemat { get; set; }
        public string? wiaTresc { get; set; }
        public DateTime? wiaData { get; set; }
    }
}

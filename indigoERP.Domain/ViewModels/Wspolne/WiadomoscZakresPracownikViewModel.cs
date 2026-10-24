using Microsoft.EntityFrameworkCore;

namespace indigoERP.Domain.ViewModels._Wspolne;

[Keyless]
public sealed class WiadomoscZakresPracownikViewModel
{
    public int? prcID { get; set; }
    public string? prcKod { get; set; }
    public string? prcImie { get; set; }
    public string? prcNazwisko { get; set; }
    public int? kntID { get; set; }
    public string? kntKontakt { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Domain.ViewModels._Wspolne;

[Table("vWiadomoscPracownik", Schema = "mvw")]
[Keyless]
public sealed class WiadomoscPracownikViewModel
{
    public int wiaID { get; set; }
    public int? wiaAplikacjaID { get; set; }
    public string? wiaEmail { get; set; }
    public string? wiaTemat { get; set; }
    public string? wiaTresc { get; set; }
    public DateTime? wiaData { get; set; }
    public string? wiaGrupa { get; set; }
    public Guid? wiaSesja { get; set; }
    public bool? wiaPubliczna { get; set; }
    public string? wiaStatus { get; set; }
    public int wprID { get; set; }
    public string? wprEmail { get; set; }
    public string? wprBlad { get; set; }
    public string? wprStatus { get; set; }
    public int prcID { get; set; }
    public string? prcKod { get; set; }
    public string? prcNazwisko { get; set; }
    public string? prcImie { get; set; }
}

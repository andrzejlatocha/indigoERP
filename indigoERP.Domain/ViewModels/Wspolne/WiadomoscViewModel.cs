using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vWiadomosc", Schema = "mvw")]
    [Keyless]
    public class WiadomoscViewModel
    {
        public int wiaID { get; set; }
        public int? wiaWiadomoscID { get; set; }
        public int? wiaAplikacjaID { get; set; }
        public int? wiaUzytkownikID { get; set; }
        public string? wiaUzytkownikNazwa { get; set; }
        public string? wiaUzytkownikImie { get; set; }
        public string? wiaUzytkownikNazwisko { get; set; }
        public string? wiaUzytkownikImieNazwisko { get; set; }
        public string? wiaEmail { get; set; }
        public string? wiaTemat { get; set; }
        public string? wiaTresc { get; set; }
        public DateTime? wiaData { get; set; }
        public string? wiaGrupa { get; set; }
        public Guid? wiaSesja { get; set; }
        public string? wiaStatus { get; set; }
        public int wuzID { get; set; }
        public int? wuzUzytkownikID { get; set; }
        public bool? wuzOdczyt { get; set; }
        public bool? wuzAkceptacja { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrImieNazwisko { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vUzytkownikLista", Schema = "mvw")]
    [Keyless]
    public class UzytkownikListaViewModel
    {
        public int usrID { get; set; }
        public int usrPracownikID { get; set; }
        public string? prcKod { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string? usrInicjal { get; set; }
        public string? usrJezyk { get; set; }
        public string? usrTelefon { get; set; }
        public string? usrKomorka { get; set; }
        public string? usrEmail { get; set; }
        public DateTime? usrDataWaznoscHaslo { get; set; }
        public bool? usrNowy { get; set; }
        public bool? usrBlokada { get; set; }
        public bool? usrEdycja { get; set; }
        public string? usrStatus { get; set; }
        public int appID { get; set; }
        public string? appFolder { get; set; }
        public string? usrFolder { get; set; }
        public string? usrZdjecie { get; set; }
        public int? wdzID { get; set; }
        public string? wdzKod { get; set; }
        public string? wdzSymbol { get; set; }
        public string? wdzNazwa { get; set; }
        public string? stnKod { get; set; }
        public string? stnSymbol { get; set; }
        public string? stnNazwa { get; set; }
        public Guid? usrGuid { get; set; }
        public bool? usrZablokowanyLogowaniem { get; set; }
    }
}

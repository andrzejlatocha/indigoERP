using Microsoft.EntityFrameworkCore;
using System;

#nullable enable

namespace indigoERP.Domain.ViewModels._Autoryzacja
{
    [Keyless]
    public class UzytkownikLoginViewModel
    {
        public int usrID { get; set; }
        public int? usrPracownikID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrInicjal { get; set; }
        public string? usrTelefon { get; set; }
        public string? usrKomorka { get; set; }
        public string? usrEmail { get; set; }
        public int? usrKlientID { get; set; }
        public int? usrDostawcaID { get; set; }
        public int? usrOdbiorcaID { get; set; }
        public int? usrSpedytorID { get; set; }
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
        public DateTime? usrDataWaznoscHaslo { get; set; }
        public string? srlID { get; set; }
        public string? srlNazwa { get; set; }
        public string? usrGuid { get; set; }
        public string? usrToken { get; set; }
        public string? sesGuid { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace indigoERP.Domain.ViewModels._Autoryzacja
{
    [PrimaryKey(nameof(usrID), nameof(appID))]
    [Table("vUzytkownik", Schema = "mvw")]
    public class UzytkownikViewModel
    {
        public int usrID { get; set; }
        public int? usrPracownikID { get; set; }
        public string? prcKod { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrInicjal { get; set; }
        public string? usrTelefon { get; set; }
        public string? usrKomorka { get; set; }
        public string? usrEmail { get; set; }
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
        public Guid? usrGuid { get; set; }
        public string? usrToken { get; set; }

        [NotMapped]
        public int? usrKlientID { get; set; }

        [NotMapped]
        public int? usrDostawcaID { get; set; }

        [NotMapped]
        public int? usrOdbiorcaID { get; set; }

        [NotMapped]
        public int? usrSpedytorID { get; set; }

        [NotMapped]
        public Guid? sesGuid { get; set; }

        [NotMapped]
        public string? usrNazwaAD { get; set; }

        [NotMapped]
        public string? usrImieNazwisko { get; set; }

        [NotMapped]
        public string? usrJezyk { get; set; }

        [NotMapped]
        public string? usrNumerKlucz { get; set; }

        [NotMapped]
        public byte[]? usrHaslo { get; set; }

        [NotMapped]
        public bool? usrNowy { get; set; }

        [NotMapped]
        public bool? usrBlokada { get; set; }

        [NotMapped]
        public bool? usrEdycja { get; set; }

        [NotMapped]
        public bool? usrSynchronizacjaAD { get; set; }

        [NotMapped]
        public Guid? usrGuidAD { get; set; }

        [NotMapped]
        public string? usrStatus { get; set; }

        [NotMapped]
        public bool? usrZablokowanyLogowaniem { get; set; }
    }
}

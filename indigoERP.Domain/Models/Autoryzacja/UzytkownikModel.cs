using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace indigoERP.Domain.Models._Autoryzacja
{
    [Keyless]
    public static class UzytkownikModel
    {
        public static int usrID { get; set; }
        public static int usrPracownikID { get; set; }
        public static string? usrNazwa { get; set; }
        public static string? usrImie { get; set; }
        public static string? usrNazwisko { get; set; }
        public static string? usrEmail { get; set; }
        public static string? appFolder { get; set; }
        public static string? usrFolder { get; set; }
        public static string? usrZdjecie { get; set; }
        public static string? wdzKod { get; set; }
        public static string? wdzSymbol { get; set; }
        public static string? wdzNazwa { get; set; }
        public static string? stnKod { get; set; }
        public static string? stnNazwa { get; set; }
        public static DateTime usrDataWaznosciHasla { get; set; }
        public static string? srlNazwa { get; set; }
        public static string? usrToken { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Administrator
{
    [Table("Pracownik", Schema = "enum")]
    public class PracownikEnumerator
    {
        [Key]
        public int prcID { get; set; }
        public string? prcKod { get; set; }
        public string? prcNazwisko { get; set; }
        public string? prcImie { get; set; }
        public string? prcOpis { get; set; }
        public string? prcZdjecie { get; set; }
        public int wdzID { get; set; }
        public string? wdzNazwaPelna { get; set; }
        public string? wdzKod { get; set; }
        public string? wdzSymbol { get; set; }
        public string? wdzNazwa { get; set; }
        public int stnID { get; set; }
        public string? stnNazwaPelna { get; set; }
        public string? stnKod { get; set; }
        public string? stnNazwa { get; set; }
    }
}

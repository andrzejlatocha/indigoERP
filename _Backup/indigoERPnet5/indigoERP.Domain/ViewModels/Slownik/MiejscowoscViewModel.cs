using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vMiejscowoscLista", Schema = "mvw")]
    public class MiejscowoscViewModel
    {
        [Key]
        public int mscID { get; set; }
        public string? mscTyp { get; set; }
        public string? mscNazwa { get; set; }
        public string? mscNazwaPelna { get; set; }
        public int kraID { get; set; }
        public string? kraNazwa { get; set; }
        public int wojID { get; set; }
        public string? wojNazwa { get; set; }
        public int powID { get; set; }
        public string? powNazwa { get; set; }
        public int gmiID { get; set; }
        public string? gmiNazwa { get; set; }
        public bool? mscPlacowkaPocztowa { get; set; }
        public string? mscStatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vPocztaLista", Schema = "mvw")]
    public class PocztaViewModel
    {
        [Key]
        public int pczID { get; set; }
        public string? pczTyp { get; set; }
        public string? pczNazwa { get; set; }
        public string? pczNazwaPelna { get; set; }
        public int kraID { get; set; }
        public string? kraNazwa { get; set; }
        public int wojID { get; set; }
        public string? wojNazwa { get; set; }
        public int powID { get; set; }
        public string? powNazwa { get; set; }
        public int gmiID { get; set; }
        public string? gmiNazwa { get; set; }
        public string? pczStatus { get; set; }
    }
}

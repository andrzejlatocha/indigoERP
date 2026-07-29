using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vGminaLista", Schema = "mvw")]
    public class GminaViewModel
    {
        [Key]
        public int gmiID { get; set; }
        public string? gmiNazwa { get; set; }
        public string? gmiNazwaPelna { get; set; }
        public int kraID { get; set; }
        public string? kraNazwa { get; set; }
        public int wojID { get; set; }
        public string? wojNazwa { get; set; }
        public int powID { get; set; }
        public string? powNazwa { get; set; }
        public string? gmiStatus { get; set; }
    }
}

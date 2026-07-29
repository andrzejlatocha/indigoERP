using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vWojewodztwoLista", Schema = "mvw")]
    public class WojewodztwoViewModel
    {
        [Key]
        public int wojID { get; set; }
        public string? wojNazwa { get; set; }
        public string? wojNazwaPelna { get; set; }
        public int kraID { get; set; }
        public string? kraNazwa { get; set; }
        public string? wojStatus { get; set; }
    }
}

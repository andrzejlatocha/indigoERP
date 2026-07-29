using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Gmina", Schema = "enum")]
    public class GminaEnumerator
    {
        [Key]
        public int gmiID { get; set; }
        public int kraID { get; set; }
        public int wojID { get; set; }
        public int powID { get; set; }
        public string gmiOpis { get; set; }
        public string gmiStatus { get; set; }
    }
}

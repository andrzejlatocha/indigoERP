using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Poczta", Schema = "enum")]
    public class PocztaEnumerator
    {
        [Key]
        public int pczID { get; set; }
        public int kraID { get; set; }
        public int wojID { get; set; }
        public int powID { get; set; }
        public int gmiID { get; set; }
        public string pczOpis { get; set; }
        public string pczStatus { get; set; }
    }
}

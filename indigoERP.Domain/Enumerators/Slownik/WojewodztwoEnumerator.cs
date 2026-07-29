using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Wojewodztwo", Schema = "enum")]
    public class WojewodztwoEnumerator
    {
        [Key]
        public int wojID { get; set; }
        public int kraID { get; set; }
        public string wojOpis { get; set; }
        public string wojStatus { get; set; }
    }
}

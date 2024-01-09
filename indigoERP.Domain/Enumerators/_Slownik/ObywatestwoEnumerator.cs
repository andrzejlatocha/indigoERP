using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Obywatelstwo", Schema = "enum")]
    public class ObywatelstwoEnumerator
    {
        [Key]
        public int obyID { get; set; }
        public string obyOpis { get; set; }
        public string obyStatus { get; set; }
    }
}

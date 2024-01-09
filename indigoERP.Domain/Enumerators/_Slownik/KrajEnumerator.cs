using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Kraj", Schema = "enum")]
    public class KrajEnumerator
    {
        [Key]
        public int kraID { get; set; }
        public string kraOpis { get; set; }
        public string kraStatus { get; set; }
    }
}

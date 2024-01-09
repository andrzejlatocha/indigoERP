using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Narodowosc", Schema = "enum")]
    public class NarodowoscEnumerator
    {
        [Key]
        public int narID { get; set; }
        public string narOpis { get; set; }
        public string narStatus { get; set; }
    }
}

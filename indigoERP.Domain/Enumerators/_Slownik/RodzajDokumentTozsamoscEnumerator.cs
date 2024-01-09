using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("RodzajDokumentTozsamosc", Schema = "enum")]
    public class RodzajDokumentTozsamoscEnumerator
    {
        [Key]
        public int dtyID { get; set; }
        public string dtyOpis { get; set; }
        public string dtyStatus { get; set; }
    }
}

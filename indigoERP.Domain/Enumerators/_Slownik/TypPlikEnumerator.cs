using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("TypPlik", Schema = "enum")]
    public class TypPlikEnumerator
    {
        [Key]
        public int tpkID { get; set; }
        public int tpkAplikacjaID { get; set; }
        public string tpkOpis { get; set; }
        public string tpkStatus { get; set; }
    }
}

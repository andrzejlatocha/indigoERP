using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("TypKontakt", Schema = "enum")]
    public class TypKontaktEnumerator
    {
        [Key]
        public int tkoID { get; set; }
        public int tkoAplikacjaID { get; set; }
        public string tkoIkona { get; set; }
        public string tkoOpis { get; set; }
        public string tkoStatus { get; set; }
    }
}

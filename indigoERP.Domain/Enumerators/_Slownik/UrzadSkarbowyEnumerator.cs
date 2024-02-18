using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("UrzadSkarbowy", Schema = "enum")]
    public class UrzadSkarbowyEnumerator
    {
        [Key]
        public string uskKOD { get; set; }
        public string uskOpis { get; set; }
        public string uskStatus { get; set; }
    }
}

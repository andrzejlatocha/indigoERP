using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Plec", Schema = "enum")]
    public class PlecEnumerator
    {
        [Key]
        public string pleKOD { get; set; }
        public string pleOpis { get; set; }
        public string pleStatus { get; set; }
    }
}

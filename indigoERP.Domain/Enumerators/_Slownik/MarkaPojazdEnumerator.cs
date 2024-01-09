using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("MarkaPojazd", Schema = "enum")]
    public class MarkaPojazdEnumerator
    {
        [Key]
        public int mpjID { get; set; }
        public int? mpjMarkaPojazdID { get; set; }
        public string? mpjOpis { get; set; }
        public string? mpjStatus { get; set; }
    }
}

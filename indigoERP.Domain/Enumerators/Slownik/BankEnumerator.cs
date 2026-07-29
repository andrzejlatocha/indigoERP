using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Bank", Schema = "enum")]
    public class BankEnumerator
    {
        [Key]
        public string? lbaKOD { get; set; }
        public string? lbaOpis { get; set; }
        public string? lbaStatus { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Godzina", Schema = "enum")]
    public class GodzinaEnumerator
    {
        [Key]
        public byte czaGodzina { get; set; }
        public string czaTekst { get; set; }
        public string czaStatus { get; set; }
    }
}

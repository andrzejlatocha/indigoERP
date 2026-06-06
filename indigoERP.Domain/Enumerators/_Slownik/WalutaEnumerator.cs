using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("Waluta", Schema = "enum")]
    public class WalutaEnumerator
    {
        [Key]
        public string walKOD { get; set; }
        public string walOpis { get; set; }
        public string walStatus { get; set; }
    }
}

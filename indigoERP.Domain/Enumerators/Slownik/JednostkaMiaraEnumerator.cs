using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("JednostkaMiara", Schema = "enum")]
    public class JednostkaMiaraEnumerator
    {
        [Key]
        public string jmiKOD { get; set; }
        public int? jmiJednostkaGrupaID { get; set; }
        public string jmiOpis { get; set; }
        public string jmiStatus { get; set; }
    }
}

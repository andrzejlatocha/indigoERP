using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class Nowosc : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]		
        public int newID { get; set; }
        public int newAplikacjaID { get; set; }
        public DateTime? newData { get; set; }
        public string newTypNowoscKOD { get; set; }
        public string newURL { get; set; }
        public byte? newWersja { get; set; }
        public string newTytul { get; set; }
        public string newOpis { get; set; }
        public string newStatus { get; set; }
    }
}

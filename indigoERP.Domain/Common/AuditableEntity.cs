using System;
using System.Linq;

namespace indigoERP.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime? DataUtworzenie { get; set; }
        public int? UtworzenieID { get; set; }
        public DateTime? DataModyfikacja { get; set; }
        public int? ModyfikacjaID { get; set; }
        public bool? Usuniety { get; set; }
    }
}

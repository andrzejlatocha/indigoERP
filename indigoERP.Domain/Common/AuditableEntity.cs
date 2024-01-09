using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime? DataUtworzenie { get; set; }
        public int? UtworzenieID { get; set; }
        public DateTime? DataModifikacja { get; set; }
        public int? ModyfikacjaID { get; set; }
        public bool? Usuniety { get; set; }
    }
}

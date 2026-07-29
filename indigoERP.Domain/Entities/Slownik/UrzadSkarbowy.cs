using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using indigoERP.Domain.Common;
#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class UrzadSkarbowy : AuditableEntity
    {
        public UrzadSkarbowy()
        {
        }

        [Key]
        public string uskKOD { get; set; }
        public string uskNazwa { get; set; }
        public string uskStatus { get; set; }
    }
}

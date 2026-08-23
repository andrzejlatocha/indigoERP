using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Common;

#nullable disable

namespace indigoERP.Domain.Entities._Podpis
{
    [Table("PodpisGrupa", Schema = "dbo")]
    public partial class PodpisGrupa : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pdgID { get; set; }
        public int pdgPodpisID { get; set; }
        public int pdgGrupaID { get; set; }
        public string pdgStatus { get; set; }

        public virtual Podpis pdgPodpis { get; set; }
    }
}

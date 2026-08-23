using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Common;

#nullable disable

namespace indigoERP.Domain.Entities._Podpis
{
    [Table("PodpisUzytkownik", Schema = "dbo")]
    public partial class PodpisUzytkownik : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pduID { get; set; }
        public int pduPodpisID { get; set; }
        public int pduUzytkownikID { get; set; }
        public string pduStatus { get; set; }

        public virtual Podpis pduPodpis { get; set; }
    }
}

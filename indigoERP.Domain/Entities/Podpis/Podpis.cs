using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Common;

#nullable disable

namespace indigoERP.Domain.Entities._Podpis
{
    [Table("Podpis", Schema = "dbo")]
    public partial class Podpis : AuditableEntity
    {
        public Podpis()
        {
            PodpisUzytkownik = new HashSet<PodpisUzytkownik>();
            PodpisGrupa = new HashSet<PodpisGrupa>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pdpID { get; set; }
        public string pdpNazwa { get; set; }
        public string pdpOpis { get; set; }
        public string pdpTresc { get; set; }
        public bool pdpDomyslny { get; set; }
        public string pdpStatus { get; set; }

        public virtual ICollection<PodpisUzytkownik> PodpisUzytkownik { get; set; }
        public virtual ICollection<PodpisGrupa> PodpisGrupa { get; set; }
    }
}

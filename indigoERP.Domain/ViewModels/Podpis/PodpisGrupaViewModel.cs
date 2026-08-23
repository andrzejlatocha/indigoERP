using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Podpis
{
    [Table("vPodpisGrupa", Schema = "mvw")]
    [Keyless]
    public class PodpisGrupaViewModel
    {
        public int pdgID { get; set; }
        public int pdgPodpisID { get; set; }
        public string pdpNazwa { get; set; }
        public int pdgGrupaID { get; set; }
        public string groNazwa { get; set; }
        public string pdgStatus { get; set; }
    }
}

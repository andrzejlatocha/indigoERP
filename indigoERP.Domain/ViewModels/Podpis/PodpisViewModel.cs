using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Podpis
{
    [Table("vPodpis", Schema = "mvw")]
    [Keyless]
    public class PodpisViewModel
    {
        public int pdpID { get; set; }
        public string pdpNazwa { get; set; }
        public string? pdpOpis { get; set; }
        public string pdpTresc { get; set; }
        public bool pdpDomyslny { get; set; }
        public string pdpStatus { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Podpis
{
    [Table("Podpis", Schema = "enum")]
    [Keyless]
    public class PodpisEnumerator
    {
        public int pdpID { get; set; }
        public string? pdpNazwa { get; set; }
        public string? pdpStatus { get; set; }
    }
}

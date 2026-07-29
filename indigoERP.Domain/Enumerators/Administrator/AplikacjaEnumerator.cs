using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Administrator
{
    [Table("Aplikacja", Schema = "enum")]
    [Keyless]
    public class AplikacjaEnumerator
    {
        public int appID { get; set; }
        public string? appNazwa { get; set; }
        public string? appStatus { get; set; }
    }
}

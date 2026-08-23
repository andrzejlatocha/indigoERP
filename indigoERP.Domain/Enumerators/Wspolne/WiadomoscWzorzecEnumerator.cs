using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Wspolne
{
    [Table("WiadomoscWzorzec", Schema = "enum")]
    [Keyless]
    public class WiadomoscWzorzecEnumerator
    {
        public int msgID { get; set; }
        public string? msgTemat { get; set; }
        public string? msgStatus { get; set; }
    }
}

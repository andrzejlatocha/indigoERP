using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("TypNowosc", Schema = "enum")]
    public class TypNowoscEnumerator
    {
        [Key]
        public string tnoKOD { get; set; }
        public string? tnoIkona { get; set; }
        public string? tnoOpis { get; set; }
        public string? tnoStatus { get; set; }
    }
}

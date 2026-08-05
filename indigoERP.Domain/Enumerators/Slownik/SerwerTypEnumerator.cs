using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Enumerators._Slownik
{
    [Table("SerwerTyp", Schema = "enum")]
    public class SerwerTypEnumerator
    {
        [Key]
        public string styKOD { get; set; }
        public string styOpis { get; set; }
        public string styStatus { get; set; }
    }
}

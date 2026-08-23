using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    [Table("JednostkaMiara", Schema = "dic")]
    public partial class JednostkaMiara
    {
        [Key]
        public string jmiKOD { get; set; }
        public int? jmiJednostkaGrupaID { get; set; }
        public string jmiOpis { get; set; }
        public string jmiStatus { get; set; }
    }
}

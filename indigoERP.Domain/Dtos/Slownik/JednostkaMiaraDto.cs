using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Slownik
{
    public class JednostkaMiaraDto
    {
        [Required]
        [StringLength(10)]
        public string jmiKOD { get; set; } = string.Empty;

        public int? jmiJednostkaGrupaID { get; set; }

        [Required]
        [StringLength(150)]
        public string jmiOpis { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        public string jmiStatus { get; set; } = "2";
    }
}

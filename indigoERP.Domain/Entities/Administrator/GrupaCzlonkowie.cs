using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Administrator
{
    [Table("GrupaCzlonkowie", Schema = "dbo")]
    public partial class GrupaCzlonkowie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int uwgID { get; set; }
        public int? uwgGrupaNadrzednaID { get; set; }
        public int? uwgUzytkownikID { get; set; }
        public int? uwgGrupaID { get; set; }
        public bool? uwgDomyslna { get; set; }
        public string? uwgStatus { get; set; }
    }
}

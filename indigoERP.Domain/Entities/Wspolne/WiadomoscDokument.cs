using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("WiadomoscDokument", Schema = "dbo")]
    public partial class WiadomoscDokument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wdoID { get; set; }
        public int? wdoWiadomoscID { get; set; }
        public string wdoTyp { get; set; }
        public string wdoNazwa { get; set; }
        public string wdoPlik { get; set; }
        public string wdoKatalog { get; set; }
        // Powiązanie z rekordem dbo.Plik (wzorzec epicaHR). Null dla załączników zapisanych przed tą zmianą.
        public int? wdoPlikID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string wdoLink { get; set; }
        public string wdoStatus { get; set; }
    }
}

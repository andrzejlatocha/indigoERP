using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vNowoscUzytkownik", Schema = "mvw")]
    [Keyless]
    public class NowoscUzytkownikViewModel
    {
        public int nusID { get; set; }
        public int? nusNowoscID { get; set; }
        public int? nusUzytkownikID { get; set; }
        public string? nusStatus { get; set; }
        public int? nowAplikacjaID { get; set; }
    }
}

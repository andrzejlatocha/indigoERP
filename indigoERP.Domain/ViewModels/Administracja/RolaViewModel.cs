using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vRola", Schema = "mvw")]
    [Keyless]
    public class RolaViewModel
    {
        public int srlID { get; set; }
        public string? srlKluczSortowania { get; set; }
        public int srlAplikacjaID { get; set; }
        public bool? srlEdycja { get; set; }
        public string srlNazwa { get; set; }
        public string? srlOpis { get; set; }
        public bool? srlBlokada { get; set; }
        public string? srlStatus { get; set; }
    }
}

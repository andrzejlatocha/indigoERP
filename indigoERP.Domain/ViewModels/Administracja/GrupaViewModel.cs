using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vGrupa", Schema = "mvw")]
    [Keyless]
    public class GrupaViewModel
    {
        public int groID { get; set; }
        public int? groTemporaryID { get; set; }
        public string? groKluczSortowania { get; set; }
        public int groAplikacjaID { get; set; }
        public string groNazwa { get; set; }
        public string? groNazwaAD { get; set; }
        public string? groOpis { get; set; }
        public DateTime? groDataWaznosc { get; set; }
        public bool? groGrupaWszyscy { get; set; }
        public bool? groAdministrator { get; set; }
        public bool? groBlokada { get; set; }
        public bool? groEdycja { get; set; }
        public bool? groSynchronizacjaAD { get; set; }
        public Guid? gruGuidAD { get; set; }
        public string? groStatus { get; set; }
    }
}

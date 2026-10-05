using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vAplikacjaUslugaUprawnienie", Schema = "mvw")]
    [Keyless]
    public class AplikacjaUslugaUprawnienieViewModel
    {
        public int usgID { get; set; }
        public int usgAplikacjaZrodloID { get; set; }
        public string? usgAplikacjaZrodloNazwa { get; set; }
        public int usgAplikacjaCelID { get; set; }
        public string? usgAplikacjaCelNazwa { get; set; }
        public string usgKodRola { get; set; }
        public bool usgAktywny { get; set; }
        public DateTime usgDataOd { get; set; }
        public DateTime? usgDataDo { get; set; }
        public string? usgOpis { get; set; }
    }
}

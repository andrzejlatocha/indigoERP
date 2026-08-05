using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vUzytkownikRolaEfektywna", Schema = "mvw")]
    [Keyless]
    public class UzytkownikRolaEfektywnaViewModel
    {
        public int cwrID { get; set; }
        public int appID { get; set; }
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string usrStatus { get; set; }
        public int srlID { get; set; }
        public string srlNazwa { get; set; }
        public string? srlOpis { get; set; }
        public string srlStatus { get; set; }
        public int uwgPoziom { get; set; }
        public bool uwgDziedziczy { get; set; }
    }
}

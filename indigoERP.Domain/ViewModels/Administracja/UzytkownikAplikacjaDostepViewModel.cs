using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vUzytkownikAplikacjaDostep", Schema = "mvw")]
    [Keyless]
    public class UzytkownikAplikacjaDostepViewModel
    {
        public int usrID { get; set; }
        public int appID { get; set; }
    }
}

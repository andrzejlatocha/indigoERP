using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vUzytkownikGrupaLista", Schema = "mvw")]
    [Keyless]
    public class UzytkownikGrupaViewModel
    {
        public int appID { get; set; }
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string usrStatus { get; set; }
        public int groID { get; set; }
        public string groNazwa { get; set; }
        public string? groNazwaAD { get; set; }
        public string? groOpis { get; set; }
        public string groStatus { get; set; }
        public int uwgPoziom { get; set; }
        public bool uwgDziedziczy { get; set; }
    }
}

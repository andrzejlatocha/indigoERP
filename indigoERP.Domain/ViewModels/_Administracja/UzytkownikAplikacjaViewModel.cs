using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vUzytkownikAplikacjaLista", Schema = "mvw")]
    [Keyless]
    public class UzytkownikAplikacjaViewModel
    {
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrNazwaAD { get; set; }
        public string usrStatus { get; set; }
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string? appOpis { get; set; }
        public string appStatus { get; set; }
        public int uwgPoziom { get; set; }
        public bool uwgDziedziczy { get; set; }
    }
}

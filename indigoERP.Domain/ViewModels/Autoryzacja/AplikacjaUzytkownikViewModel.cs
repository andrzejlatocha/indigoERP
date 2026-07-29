using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace indigoERP.Domain.ViewModels._Autoryzacja
{
    [Table("vUzytkownikAplikacja", Schema = "app")]
    public class AplikacjaUzytkownikViewModel
    {
        [Key]
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string appWersja { get; set; }
        public string appOpis { get; set; }
        public string appIkona { get; set; }
        public string appLink { get; set; }
        public string appFolder { get; set; }
        public int usrID { get; set; }

    }
}

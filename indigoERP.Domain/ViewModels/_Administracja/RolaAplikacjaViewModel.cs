using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vRolaAplikacjaLista", Schema = "mvw")]
    [Keyless]
    public class RolaAplikacjaViewModel
    {
        public int srlID { get; set; }
        public string srlNazwa { get; set; }
        public string? srlOpis { get; set; }
        public string srlStatus { get; set; }
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string? appOpis { get; set; }
        public string appStatus { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vGrupaAplikacjaLista", Schema = "mvw")]
    [Keyless]
    public class GrupaAplikacjaViewModel
    {
        public int groID { get; set; }
        public string groNazwa { get; set; }
        public string? groNazwaAD { get; set; }
        public string? groOpis { get; set; }
        public string groStatus { get; set; }
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string? appOpis { get; set; }
        public string appStatus { get; set; }
        public int uwgPoziom { get; set; }
        public bool uwgDziedziczy { get; set; }
    }
}

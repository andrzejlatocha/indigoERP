using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.ViewModels._Administracja
{
    [Table("vGrupaRolaLista", Schema = "mvw")]
    [Keyless]
    public class GrupaRolaViewModel
    {
        public int cwrID { get; set; }
        public int appID { get; set; }
        public int groID { get; set; }
        public string groNazwa { get; set; }
        public string? groNazwaAD { get; set; }
        public string? groOpis { get; set; }
        public string groStatus { get; set; }
        public int srlID { get; set; }
        public string srlNazwa { get; set; }
        public string? srlOpis { get; set; }
        public string srlStatus { get; set; }
        public int uwgPoziom { get; set; }
        public bool uwgDziedziczy { get; set; }
    }
}

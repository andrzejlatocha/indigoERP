using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace indigoERP.Domain.ViewModels
{
    [Table("vAplikacjaLista", Schema = "mvw")]
    public class AplikacjaViewModel
    {
        [Key]
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string? appWersja { get; set; }
        public string? appOpis { get; set; }
        public string? appLink { get; set; }
        public string? appIkona { get; set; }
        public string? appFolder { get; set; }
        public bool appWyswietl { get; set; }
        public string appStatus { get; set; }
    }
}

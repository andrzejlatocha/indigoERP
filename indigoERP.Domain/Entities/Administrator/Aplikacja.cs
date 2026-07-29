using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Common;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Aplikacja : AuditableEntity
    {
        public Aplikacja()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string appWersja { get; set; }
        public string appOpis { get; set; }
        public string appLink { get; set; }
        public string appIkona { get; set; }
        public string appFolder { get; set; }
        public bool? appWyswietl { get; set; }
        public string appStatus { get; set; }
    }
}

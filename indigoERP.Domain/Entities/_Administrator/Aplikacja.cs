using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Aplikacja
    {
        public Aplikacja()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int appID { get; set; }
        public string appNazwa { get; set; }
        public string appWersja { get; set; }
        public string appOpis { get; set; }
        public string appIkona { get; set; }
        public string appStatus { get; set; }
    }
}

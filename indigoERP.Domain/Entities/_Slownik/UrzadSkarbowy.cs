using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class UrzadSkarbowy
    {
        public UrzadSkarbowy()
        {
        }

        [Key]
        public string uskKOD { get; set; }
        public string uskNazwa { get; set; }
        public string uskStatus { get; set; }
    }
}

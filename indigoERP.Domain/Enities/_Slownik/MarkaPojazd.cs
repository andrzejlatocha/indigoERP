using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Enities._Slownik
{
    public class MarkaPojazd
    {
        [Key]
        public int mpjID { get; set; }
        public int? mpjMarkaPojazdID { get; set; }
        public string? mpjNazwa { get; set; }
        public string? mpjStatus { get; set; }
    }
}

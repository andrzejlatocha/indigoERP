using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    public class MarkaPojazdViewModel
    {
        [Key]
        public int mpjID { get; set; }
        public string? mpjNazwa { get; set; }
        public string? mpjStatus { get; set; }
    }
}
